namespace Ecommerce.Products.Application.Services.Internals;

using Microsoft.EntityFrameworkCore;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Products.Domain.Enums;
using Ecommerce.Products.Application.Common;

using Ecommerce.Shared.Database;
using Ecommerce.Shared.Common.Extensions;

/// <summary>
/// Application service responsible for orchestrating bulk product imports from Excel file streams.
/// Performs memory-efficient entity lookups, row-level validations, and transactional database persistence.
/// </summary>
public class ProductImportService(AppDbContext context, IProductExcelParser excelParser) : IProductImportService
{
    private readonly AppDbContext _context = context;
    private readonly IProductExcelParser _excelParser = excelParser;

    public async Task<ProductImportResultDto> ImportFromExcelAsync(
        Stream fileStream, 
        CancellationToken cancellationToken = default)
    {
        // Step 1: Parse raw Excel stream into Application DTOs
        List<ProductImportDto> rows = _excelParser.ParseExcel(fileStream).ToList();

        if (rows.Count == 0)
        {
            return new ProductImportResultDto(
                TotalRecords: 0,
                SuccessfulCount: 0,
                FailedCount: 0,
                Errors: ["The uploaded Excel file contains no data rows."]
            );
        }

        var errors = new List<string>();

        // Step 2: Pre-fetch existing lookups to optimize database roundtrips
        var existingCategories = await _context.Set<Category>().ToListAsync(cancellationToken);
        var existingBrands = await _context.Set<Brand>().ToListAsync(cancellationToken);

        // Build in-memory lookup maps keyed by Slug
        var rootCategoryMap = existingCategories
            .Where(c => c.ParentCategoryId == null)
            .GroupBy(c => c.Slug)
            .ToDictionary(g => g.Key, g => g.First());

        var subcategoryMap = existingCategories
            .Where(c => c.ParentCategoryId != null)
            .GroupBy(c => $"{c.ParentCategoryId}_{c.Slug}")
            .ToDictionary(g => g.Key, g => g.First());

        var brandMap = existingBrands
            .GroupBy(b => b.Slug)
            .ToDictionary(g => g.Key, g => g.First());

        // NOTE: Declare the dictionary OUTSIDE the loop that iterates through the Excel file
        var productsMap = new Dictionary<string, Product>();

        // Step 3: Process rows sequentially (Row 1 is usually header, so row index starts at 2)
        int rowIndex = 1;

        foreach (var row in rows)
        {
            rowIndex++;

            // Row Sanitation
            if (string.IsNullOrWhiteSpace(row.Name))
            {
                errors.Add($"Row {rowIndex}: Product name is required.");
                continue;
            }

            // Validate Price
            if (!row.PriceArs.HasValue || row.PriceArs.Value <= 0)
            {
                errors.Add($"Row {rowIndex}: Price (price_ars) is required and must be greater than zero.");
                continue;
            }

            // Validate Unit Cost
            if (!row.UnitCostArs.HasValue || row.UnitCostArs.Value <= 0)
            {
                errors.Add($"Row {rowIndex}: Unit Cost (cost_ars) is required and must be greater than zero.");
                continue;
            }

            // Validate Stock
            if (!row.Stock.HasValue || row.Stock.Value < 0)
            {
                errors.Add($"Row {rowIndex}: Stock is required and must be a non-negative integer.");
                continue;
            }

            // 1. Resolve or Create Category Hierarchy, abd Subcategory
            var (rootCategory, subcategory) = ResolveCategories(
                row.Category, 
                row.Subcategory, 
                rootCategoryMap, 
                subcategoryMap);

            // 2. Resolve or Create Brand
            var brand = ResolveBrand(row.Brand, brandMap);

            // 3. Build Product Entity & Associated Child Collections
            var slug = row.Name.ToSlug();

            // If it has a GroupProduct, we use that value as the key.
            // If it doesn't have a group, we use the unique slug so that each row is an independent product.
            var productKey = !string.IsNullOrWhiteSpace(row.GroupProduct) 
                ? row.GroupProduct.Trim() : slug;


            if (!productsMap.TryGetValue(productKey, out var product))
            {
                product = new Product
                {
                    Name = row.Name,
                    Slug = slug,
                    MainImageUrl = row.ImageUrl,
                    Description = row.Description,
                    IsActive = row.IsActive,
                    Category = rootCategory,
                    Subcategory = subcategory,
                    Brand = brand
                };
                productsMap[productKey] = product;
            }

            // Default SKU Variant
            string nameVariant = ProductVariantUtils.BuildDisplayName(
                row.Name, row.Size, row.Color, row.ColorName);
            string normalizedName = ProductVariantUtils.BuildNormalizedName(
                row.Name, row.Size, row.Color, row.ColorName);
            
            var variant = new ProductVariant
            {
                Name = nameVariant,
                NormalizedName = normalizedName,
                SKU = ProductVariantUtils.GenerateSku(),
                MainImageUrl = row.ImageUrl,
                IsActive = true,

                Size = row.Size,
                Color = ColorExtensions.ToBaseColor(row.Color),        // get BaseColor value
                DisplayColorName = row.ColorName,
                HexColor = ColorExtensions.ResolveHexColor(row.Color),

                Stock = row.Stock.Value,
                PriceArs = row.PriceArs.Value,
                UnitCostArs = row.UnitCostArs.Value,
                DiscountPercentageArs = row.DiscountPercentageArs ?? 0
            };
            product.Variants.Add(variant);

            // * Product Media Gallery
            // Attach to the specific variant, but deduplicate by URL across the whole product
            AttachVariantImage(product, variant, row.ImageUrl, isMainImage: true, displayOrder: 1);
            AttachVariantImage(product, variant, row.ImageUrl2, isMainImage: false, displayOrder: 2);
        }

        // 3. Obtenemos la lista final de productos listos para persistir en EF Core
        var productsToSave = productsMap.Values.ToList();

        // Step 4: Batch Transactional Save
        if (productsToSave.Count > 0)
        {
            // Step 4.1: Begin database transaction scope
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Step 4.2: Attach entities to the ChangeTracker in batch
                await _context.Set<Product>().AddRangeAsync(productsToSave, cancellationToken);

                // Step 4.3: Execute SQL insert queries for products and navigation graphs
                await _context.SaveChangesAsync(cancellationToken);

                // Step 4.4: Commit the active transaction
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Step 4.5: Revert all database changes if insertion fails
                await transaction.RollbackAsync(cancellationToken);
                errors.Add($"Database transaction failed during bulk insert: {ex.Message}");
                return new ProductImportResultDto(
                    TotalRecords: rows.Count,
                    SuccessfulCount: 0,
                    FailedCount: rows.Count,
                    Errors: errors
                );
            }
        }

        // Step 5: Construct final execution result payload
        return new ProductImportResultDto(
            TotalRecords: rows.Count,
            SuccessfulCount: productsToSave.Count,
            FailedCount: errors.Count,
            Errors: errors
        );
    }

    // * =========================================================================
    // * PRIVATE HELPER METHODS
    // * =========================================================================

    /// <summary>
    /// Resolves the root category and subcategory entities using in-memory slug maps.
    /// Instantiates and tracks new <see cref="Category"/> entities if they do not exist in the catalog.
    /// </summary>
    /// <param name="cleanCategory">The pre-sanitized display name of the primary category, or <c>null</c> if unassigned.</param>
    /// <param name="cleanSubcategory">The pre-sanitized display name of the subcategory, or <c>null</c> if unassigned.</param>
    /// <param name="rootMap">An in-memory lookup dictionary mapping root category slugs to their tracked <see cref="Category"/> entities.</param>
    /// <param name="subMap">An in-memory lookup dictionary mapping contextual keys (<c>"{rootSlug}_{subSlug}"</c>) to their tracked subcategory <see cref="Category"/> entities.</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    ///   <item><description><c>Root</c>: The resolved or newly instantiated root <see cref="Category"/> entity.</description></item>
    ///   <item><description><c>Sub</c>: The resolved or newly instantiated child <see cref="Category"/> entity linked to <c>Root</c>.</description></item>
    /// </list>
    /// </returns>
    private static (Category? Root, Category? Sub) ResolveCategories(
        string? cleanCategory,
        string? cleanSubcategory,
        Dictionary<string, Category> rootMap,
        Dictionary<string, Category> subMap)
    {
        // Step 1: Early Exit Guard
        // If the primary category input is null, empty, or whitespace, abort immediately 
        // and return null references without logging false errors.
        if (string.IsNullOrWhiteSpace(cleanCategory))
            return (null, null);

        // Step 2: Generate URL-friendly slug for root category lookup
        var rootSlug = cleanCategory.ToSlug();

        // Step 3: Resolve or Instantiate Root Category
        // Query in-memory cache using the slug key. If missing, create a new Category entity,
        // default its status to active, and store it in the dictionary to track across subsequent rows.
        if (!rootMap.TryGetValue(rootSlug, out var rootCategory))
        {
            rootCategory = new Category
            {
                Name = cleanCategory,
                Slug = rootSlug,
                IsActive = true
            };

            rootMap[rootSlug] = rootCategory;
        }

        // Step 4: Resolve or Instantiate Subcategory (Conditional Execution)
        Category? subcategory = null;

        if (!string.IsNullOrWhiteSpace(cleanSubcategory))
        {
            var subSlug = cleanSubcategory.ToSlug();
            
            // Construct contextual composite key to isolate subcategories per parent category slug 
            // (prevents key collisions when two different root categories share subcategory names like "Accesorios").
            var subMapKey = $"{rootSlug}_{subSlug}";

            // Search in-memory subcategory map. If missing, instantiate child Category,
            // establish entity relationship graph via ParentCategory, and track in dictionary.
            if (!subMap.TryGetValue(subMapKey, out subcategory))
            {
                subcategory = new Category
                {
                    Name = cleanSubcategory,
                    Slug = subSlug,
                    IsActive = true,
                    ParentCategory = rootCategory
                };

                subMap[subMapKey] = subcategory;
            }
        }

        // Step 5: Return tuple containing tracked entity instances ready for EF Core graph persistence
        return (rootCategory, subcategory);
    }

    /// <summary>
    /// Resolves a <see cref="Brand"/> entity from the in-memory slug lookup map, 
    /// or instantiates and tracks a new <see cref="Brand"/> entity if it does not exist in the catalog.
    /// </summary>
    /// <param name="cleanBrand">The pre-sanitized display name of the brand, or <c>null</c> if unassigned.</param>
    /// <param name="brandMap">An in-memory dictionary mapping brand slugs to their tracked <see cref="Brand"/> instances.</param>
    /// <returns>
    /// The resolved or newly created <see cref="Brand"/> entity ready for persistence, 
    /// or <c>null</c> if the input value was empty or omitted.
    /// </returns>
    private static Brand? ResolveBrand(string? cleanBrand, Dictionary<string, Brand> brandMap)
    {
        // Step 1: Early Exit Guard
        // If the input brand string is null, empty, or whitespace, abort immediately and return null.
        if (string.IsNullOrWhiteSpace(cleanBrand))
            return null;

        // Step 2: Generate URL-friendly slug for brand lookup
        var brandSlug = cleanBrand.ToSlug();

        // Step 3: Resolve or Instantiate Brand
        // Search the in-memory lookup map using the slug key. If missing, instantiate a new Brand entity,
        // default its status to active, and cache it in the dictionary to ensure reuse across subsequent rows.
        if (!brandMap.TryGetValue(brandSlug, out var brand))
        {
            brand = new Brand
            {
                Name = cleanBrand,
                Slug = brandSlug,
                IsActive = true
            };

            brandMap[brandSlug] = brand;
        }

        // Step 4: Return tracked Brand instance ready for EF Core navigation property assignment
        return brand;
    }

    /// <summary>
    /// Attaches a new <see cref="ProductImage"/> asset to the master product gallery and associates it with a specific variant.
    /// Performs in-memory deduplication against the existing <see cref="Product.Images"/> navigation graph.
    /// </summary>
    /// <param name="product">The parent master product entity holding the image collection.</param>
    /// <param name="variant">The specific product variant entity to associate with this image asset.</param>
    /// <param name="cleanUrl">The pre-sanitized image URL link.</param>
    /// <param name="isMainImage">Indicates whether this image is the primary cover photo. Defaults to <c>false</c>.</param>
    /// <param name="displayOrder">The sorting position index for gallery rendering. Defaults to <c>1</c>.</param>
    private static void AttachVariantImage(
        Product product, 
        ProductVariant variant, 
        string? cleanUrl, 
        bool isMainImage, 
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(cleanUrl))
            return;

        // NOTE FOR ME:
        // 'product.Images' is a local, in-memory ICollection<ProductImage> instantiated during processing.
        // Calling .Any() here executes pure LINQ-to-Objects against local RAM — it DOES NOT hit or query the database.
        if (!product.Images.Any(img => img.Url == cleanUrl))
        // We only add it if the Master Product doesn't already have this URL registered.
        // This prevents sizes S, M, and L from saving the same repeated photo.
        {
            product.Images.Add(new ProductImage
            {
                Url = cleanUrl,
                IsMainImage = isMainImage,
                DisplayOrder = displayOrder,
                // EF Core automatically maps the ProductVariantId relationship upon SaveChangesAsync
                ProductVariant = variant 
            });
        }
    }
}
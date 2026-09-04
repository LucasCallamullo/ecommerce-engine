namespace Ecommerce.Products.Application.Services.Internals;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Domain.Entities;

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

    /// <inheritdoc/>
    public async Task<ProductImportResultDto> ImportFromExcelAsync(
        Stream fileStream, 
        CancellationToken cancellationToken = default)
    {
        // Step 1: Parse raw Excel stream into Application DTOs
        var rows = _excelParser.ParseExcel(fileStream).ToList();

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
        var productsToInsert = new List<Product>();

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

        // Step 3: Process rows sequentially (Row 1 is usually header, so row index starts at 2)
        int rowIndex = 1;

        foreach (var row in rows)
        {
            rowIndex++;

            // Row Sanitation
            var cleanName = row.Name.Sanitize();
            if (string.IsNullOrWhiteSpace(cleanName))
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
            var slug = cleanName.ToSlug();

            var product = new Product
            {
                Name = cleanName,
                Slug = slug,
                Description = row.Description.Sanitize(),
                Category = rootCategory,
                Subcategory = subcategory,
                IsActive = row.Available,
                Brand = brand
            };

            // Default SKU Variant
            product.Variants.Add(new ProductVariant
            {
                SKU = $"{slug.ToUpperInvariant()}-DEFAULT",
                Stock = row.Stock.Value,
                IsActive = true,
                PriceArs = row.PriceArs.Value,
                DiscountArs = row.DiscountArs ?? 0
            });

            // Product Media Gallery
            var primaryImageUrl = row.ImageUrl.Sanitize();
            if (!string.IsNullOrWhiteSpace(primaryImageUrl))
            {
                product.Images.Add(new ProductImage
                {
                    Url = primaryImageUrl,
                    IsMainImage = true,
                    DisplayOrder = 1
                });

                // ! TODO , no agrupa en memoria el valor ver despues
                product.MainImageUrl = primaryImageUrl;
            }

            var secondaryImageUrl = row.ImageUrl2.Sanitize();
            if (!string.IsNullOrWhiteSpace(secondaryImageUrl))
            {
                product.Images.Add(new ProductImage
                {
                    Url = secondaryImageUrl,
                    IsMainImage = false,
                    DisplayOrder = 2
                });
            }

            productsToInsert.Add(product);
        }

        // Step 4: Batch Transactional Save
        if (productsToInsert.Count > 0)
        {
            // Step 4.1: Begin database transaction scope
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Step 4.2: Attach entities to the ChangeTracker in batch
                await _context.Set<Product>().AddRangeAsync(productsToInsert, cancellationToken);

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
            SuccessfulCount: productsToInsert.Count,
            FailedCount: errors.Count,
            Errors: errors
        );
    }

    // * =========================================================================
    // * PRIVATE HELPER METHODS
    // * =========================================================================

    /// <summary>
    /// Resolves root category and subcategory from in-memory maps or instantiates new Category entities if missing.
    /// </summary>
    private static (Category? Root, Category? Sub) ResolveCategories(
        string? rawCategory,
        string? rawSubcategory,
        Dictionary<string, Category> rootMap,
        Dictionary<string, Category> subMap)
    {
        var cleanCategory = rawCategory.Sanitize();

        // If root category cell is empty, return nulls without logging an error
        if (string.IsNullOrWhiteSpace(cleanCategory))
            return (null, null);

        var rootSlug = cleanCategory.ToSlug();

        // 1. Resolve or Instantiate Root Category
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

        // 2. Resolve or Instantiate Subcategory (If provided)
        Category? subcategory = null;
        var cleanSubcategory = rawSubcategory.Sanitize();

        if (!string.IsNullOrWhiteSpace(cleanSubcategory))
        {
            var subSlug = cleanSubcategory.ToSlug();
            var subMapKey = $"{rootSlug}_{subSlug}"; // Contextual key per parent

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

        return (rootCategory, subcategory);
    }

    /// <summary>
    /// Resolves a brand from the in-memory slug map or instantiates a new Brand entity if missing.
    /// </summary>
    private static Brand? ResolveBrand(string? rawBrand, Dictionary<string, Brand> brandMap)
    {
        var cleanBrand = rawBrand.Sanitize();
        if (string.IsNullOrWhiteSpace(cleanBrand))
            return null;

        var brandSlug = cleanBrand.ToSlug();

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

        return brand;
    }
}
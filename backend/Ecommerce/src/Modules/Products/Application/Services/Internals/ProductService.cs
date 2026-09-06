namespace Ecommerce.Products.Application.Services.Internals;

using Mapster;
using System.Net;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Common.Extensions;

public class ProductService(
    AppDbContext context, 
    IBrandService brandService, 
    ICategoryService categoryService, 
    IVariantService variantService) : IProductService
{
    private readonly AppDbContext _context = context;
    private readonly IBrandService _brandService = brandService;
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IVariantService _variantService = variantService;
    

    //? =====================================================================
    //?             Entity METHODS
    //? =====================================================================

    public async Task<T> GetEntityByIdAsync<T>(
        int id, 
        Expression<Func<Product, T>> selector, 
        CancellationToken ct = default)
    {
        return await _context.Set<Product>()
            .AsNoTracking()
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(selector)
            .FirstOrDefaultAsync(ct)
            ?? throw new AppException($"Product with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<Product> GetEntityByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Product>()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken) ??
            throw new AppException($"Product with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _context.Set<Product>()
            .AsNoTracking()
            .AnyAsync(p => p.Id == id && !p.IsDeleted, ct);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Product, bool>> predicate, 
        CancellationToken ct = default)
    {
        return await _context.Set<Product>()
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .AnyAsync(predicate, ct);
    }

    //? =====================================================================
    //?             GET METHODS
    //? =====================================================================

    public async Task<ProductDetailResponse> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var product = await _context.Set<Product>()
            .AsNoTracking()
            .ProjectToType<ProductDetailResponse>()
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive, ct)
            ?? throw new AppException($"Product with slug '{slug}' was not found.", HttpStatusCode.NotFound);

        return product;
    }

    public async Task<ProductDetailResponse> GetByIdDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        // Step 1: Query DbSet<Product> with Change Tracking disabled.
        // Step 2: Filter by primary key.
        // Step 3: Project directly into ProductDetailResponse. Mapster automatically detects 
        //        (Category, Subcategory, Brand, Variants) property and creates an SQL LEFT JOIN 
        //         to select (Category, Subcategory, Brand, Variants) columns.
        // Step 4: Asynchronously return the projected detail DTO or null.
        return await _context.Set<Product>()
            .AsNoTracking()
            .Where(p => p.Id == id && !p.IsDeleted)
            .ProjectToType<ProductDetailResponse>()
            .FirstOrDefaultAsync(cancellationToken) ?? 
            throw new AppException($"Product with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<ProductResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Step 1: Query the DbSet<Product> with Change Tracking disabled for read-only evaluation.
        // Step 2: Apply a SQL WHERE clause filter targeting the primary key (p.Id == id).
        // Step 3: Project the filtered entity columns into the ProductResponse DTO structure at the SQL level.
        // Step 4: Asynchronously fetch the first matching record or return null if no row matches the condition.
        return await _context.Set<Product>()
            .AsNoTracking()
            .Where(p => p.Id == id && !p.IsDeleted)
            .ProjectToType<ProductResponse>()
            .FirstOrDefaultAsync(cancellationToken) ?? 
            throw new AppException($"Product with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Step 1: Query the DbSet<Product> without registering entities in the DbContext Change Tracker (improves read performance).
        // Step 2: Project only the properties defined in ProductResponse directly into SQL SELECT clause via Mapster.
        // Step 3: Asynchronously execute the query against SQLite and return the mapped collection as a List.
        return await _context.Set<Product>()
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .ProjectToType<ProductResponse>()
            .ToListAsync(cancellationToken);
    }

    //? =====================================================================
    //?         Methods --> Create | Update | Delete 
    //? =====================================================================

    public async Task<ProductDetailResponse> CreateAsync(
        ProductCreateRequest request, 
        CancellationToken ct = default)
    {
        // Step 1: Validate & resolve all FK relationships (Throws 404 if invalid)
        var brand = request.BrandId.HasValue 
            ? await _brandService.GetEntityByIdAsync(request.BrandId.Value, ct) 
            : null;

        var category = request.CategoryId.HasValue 
            ? await _categoryService.GetEntityByIdAndParentAsync(request.CategoryId.Value, parentId: null, ct) 
            : null;

        var subcategory = (request.CategoryId.HasValue && request.SubcategoryId.HasValue) 
            ? await _categoryService.GetEntityByIdAndParentAsync(
                request.SubcategoryId.Value, 
                parentId: request.CategoryId.Value, 
                ct) 
            : null;

        // Step 2: Map scalar properties and generate domain values
        var product = request.Adapt<Product>();
        product.Slug = request.Name.ToSlug();

        // Step 4: Map and associate child variants using the dedicated domain service.
        product.Variants = _variantService.CreateVariantsFromRequests(request.Variants, product);

        // Step 5: Add the root aggregate to the DbContext tracking graph.
        // Both Product and its child Variants are persisted atomically in a single database round-trip.
        _context.Set<Product>().Add(product);
        await _context.SaveChangesAsync(ct);

        // Step 5: Map persisted entity to DTO and enrich with resolved relation metadata
        var response = product.Adapt<ProductDetailResponse>();

        return response with
        {
            Brand = brand?.Adapt<BrandResponse>(),
            Category = category?.Adapt<CategoryResponse>(),
            Subcategory = subcategory?.Adapt<CategoryResponse>()
        };
    }

    public async Task<ProductResponse> UpdateAsync(
        int id,
        ProductUpdateRequest request, 
        CancellationToken ct = default)
    {
        // Step 1: Retrieve tracked entity (Ensures 404 if not found)
        var product = await GetEntityByIdAsync(id, ct);

        // Step 2: Validate Brand Foreign Key if changed
        if (request.BrandId != product.BrandId)
        {
            var brand = request.BrandId.HasValue
                ? await _brandService.GetEntityByIdAsync(request.BrandId.Value, ct)
                : null;

            product.BrandId = brand?.Id;
        }

        // Determine target Category & Subcategory IDs for valid hierarchy check
        var targetCategoryId = request.CategoryId ?? product.CategoryId;
        var targetSubcategoryId = request.SubcategoryId ?? product.SubcategoryId;

        // Step 3: Validate Category Foreign Key if changed
        if (request.CategoryId != product.CategoryId)
        {
            var category = targetCategoryId.HasValue
                ? await _categoryService.GetEntityByIdAndParentAsync(targetCategoryId.Value, parentId: null, ct)
                : null;

            product.CategoryId = category?.Id;
        }

        // Step 4: Validate Subcategory Foreign Key if changed
        if (request.SubcategoryId != product.SubcategoryId)
        {
            var subcategory = (targetCategoryId.HasValue && targetSubcategoryId.HasValue)
                ? await _categoryService.GetEntityByIdAndParentAsync(
                    targetSubcategoryId.Value, 
                    parentId: targetCategoryId.Value, 
                    ct)
                : null;

            product.SubcategoryId = subcategory?.Id;
        }

        // Step 5: Update Slug if Name changed
        if (!string.IsNullOrWhiteSpace(request.Name) && product.Name != request.Name)
        {
            product.Slug = request.Name.ToSlug();
        }

        // Step 6: Map scalar properties onto the tracked entity
        request.Adapt(product);

        // Step 7: Persist changes (EF Core ChangeTracker updates modified columns only)
        await _context.SaveChangesAsync(ct);

        // Step 8: Return lightweight summary response (avoids unnecessary DB joins)
        return product.Adapt<ProductResponse>();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        // Step 1. Query Execution: Retrieve product along with its related active variants
        var entity = await _context.Set<Product>()
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id, ct) 
            ?? throw new AppException($"Product with ID {id} was not found.", HttpStatusCode.NotFound);

        // Step 2. Domain Validation: Prevent redundant soft deletion operations
        if (entity.IsDeleted) 
            throw new AppException($"Product with ID {id} is already deleted.", HttpStatusCode.BadRequest);

        // Step 3. Parent Soft Delete: Flag parent product entity as deleted
        entity.IsDeleted = true;

        // Step 4. Cascade Soft Delete: Flag all associated child variants as deleted
        foreach (var variant in entity.Variants)
        {
            variant.IsDeleted = true;
        }

        // Step 5. Persistence: Execute single database transaction updating product and variants
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
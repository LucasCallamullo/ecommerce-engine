using System.Net;
using Mapster;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Domain.Entities;

namespace Ecommerce.Products.Application.Services.Internals;

public class ProductService(
    AppDbContext context, 
    IVariantService variantService) : IProductService
{
    private readonly AppDbContext _context = context;
    private readonly IVariantService _variantService = variantService;

    //? =====================================================================
    //?             Entity METHODS
    //? =====================================================================

    public async Task<Product> GetEntityByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Product>()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken) ??
            throw new AppException($"Product with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    //? =====================================================================
    //?             GET METHODS
    //? =====================================================================

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

    public async Task<ProductResponse> CreateAsync(
        ProductCreateRequest request, 
        CancellationToken cancellationToken = default)
    {
        // Step 1: Maps scalar properties (ProductMappingConfig automatically ignores Variants mapping)
        var product = request.Adapt<Product>();

        // Step 2: Generate and assign a URL-friendly slug based on the product name.
        product.Slug = GenerateSlug(request.Name);

        // Step 4: Map and associate child variants using the dedicated domain service.
        product.Variants = _variantService.CreateVariantsFromRequests(request.Variants, product);

        // Step 5: Add the root aggregate to the DbContext tracking graph.
        // Both Product and its child Variants are persisted atomically in a single database round-trip.
        _context.Set<Product>().Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        // Step 6: Map the newly persisted entity (including database-generated IDs) to the response DTO.
        return product.Adapt<ProductResponse>();
    }

    public async Task<ProductResponse> UpdateAsync(
        int id,
        ProductUpdateRequest request, 
        CancellationToken cancellationToken = default)
    {
        // 1. get Entity (tracked) by EF Core
        var product = await GetEntityByIdAsync(id, cancellationToken);

        // TODO: Validate existence of BrandId, CategoryId, and SubcategoryId if provided
        // TODO: Validar existencia de BrandId (si viene en la request)
        // TODO: Validar existencia de CategoryId y SubcategoryId (si vienen en la request)

        // 2. If name changes update slug.
        if (request.Name is not null && product.Name != request.Name)
            product.Slug = GenerateSlug(request.Name);

        // 3. Maps non-null properties to tracked entity (ProductMappingConfig handles null values and FKs)
        request.Adapt(product);

        // 4. Save changes (EF Core only detects modified properties)
        await _context.SaveChangesAsync(cancellationToken);

        // 5. Project to response DTO and return
        return product.Adapt<ProductResponse>();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // Step 1. Query Execution: Retrieve product along with its related active variants
        var entity = await _context.Set<Product>()
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken) 
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
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    // Helper method to convert product names into URL-safe slugs
    private static string GenerateSlug(string name)
    {
        return name
            .ToLowerInvariant()    // Step 1: Normalize case to lower invariant.
            .Trim()                // Step 2: Trim leading/trailing whitespaces.
            .Replace(" ", "-")
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")    // Step 3: Replace spaces with hyphens.
            .Replace("ñ", "n")    // Step 4: Normalize Spanish special characters/accents to ASCII equivalents.
            .Replace("ü", "u")
            .Replace("'", "")
            .Replace("\"", "");    // Step 5: Remove quote characters to clean the URL path.
    }
}
using System.Net;
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Products.Application.Services;

public class ProductService(
    AppDbContext context, 
    IVariantService variantService) : IProductService
{
    private readonly AppDbContext _context = context;
    private readonly IVariantService _variantService = variantService;

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

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request, 
        CancellationToken cancellationToken = default)
    {
        // Step 1: Configure Mapster to ignore navigation collections during initial mapping.
        // This prevents automatic, unmanaged mapping of child items like Variants.
        var config = new TypeAdapterConfig();
        config.NewConfig<CreateProductRequest, Product>()
            .Ignore(dest => dest.Variants);

        // Step 2: Map scalar properties from the request DTO to a new Product entity instance.
        var product = request.Adapt<Product>(config);

        // Step 3: Generate and assign a URL-friendly slug based on the product name.
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


    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // Step 1: Search local DbContext memory first; if absent, execute a SQL query to find the tracked entity by primary key.
        // var entity = await _context.Set<Domain.Entities.Product>()
        //    .FindAsync(new object[] { id }, cancellationToken);

        var entity = await _context.Set<Product>()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken) ??
            throw new AppException($"Product with ID {id} was not found.", HttpStatusCode.NotFound);

        // Step 3: Implementation of Soft Delete
        entity.IsDeleted = true;
        _context.Set<Product>().Update(entity);

        // Step 4: Commit changes to SQLite, executing a SQL DELETE command within an implicit transaction.
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
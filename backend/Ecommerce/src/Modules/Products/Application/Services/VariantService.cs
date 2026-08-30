// Ecommerce.Product.Application/Services/VariantService.cs
using System.Net;
using Mapster;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;

namespace Ecommerce.Products.Application.Services;

public class VariantService(AppDbContext context) : IVariantService
{
    private readonly AppDbContext _context = context;

    // Maps a collection of variant request DTOs into domain entities attached to a parent Product.
    public List<ProductVariant> CreateVariantsFromRequests(
        List<ProductCreateVariantRequest>? variantRequests, 
        Product product)
    {
        // Step 1: Validate input collection; return an empty list if null or empty.
        if (variantRequests is null || variantRequests.Count == 0) 
            return [];

        // Step 2: Project each request DTO to a ProductVariant entity.
        return variantRequests.Select(v => new ProductVariant
        {
            SKU = v.SKU ?? GenerateSku(),
            PriceArs = v.PriceArs,
            ComparisonPriceArs = v.ComparisonPriceArs,
            DiscountArs = v.DiscountArs,
            Stock = v.Stock,
            Size = v.Size,
            Color = v.Color,
            HexColor = v.HexColor,
            // Step 4: Establish the bi-directional navigation reference with the parent Product.
            Product = product 
        }).ToList();
    }

    // Helper method to generate a fallback unique SKU identifier.
    public string GenerateSku()
    {
        // Step 1: Generate a pseudo-random 4-digit numerical suffix using thread-safe Random.Shared.
        var random = Random.Shared.Next(1000, 9999).ToString();
        var random2 = Random.Shared.Next(1000, 9999).ToString();
        return $"SKU-{random}-{random2}";
    }
    
    // ? CRUD METHODS

    public async Task<ProductVariantResponse> CreateAsync(
        int productId,
        ProductCreateVariantRequest request, 
        CancellationToken cancellationToken = default)
    {
        // Step 1: Ensure parent product exists, this for circular dependency
        await EnsureProductExistsAsync(productId, cancellationToken);
       
        // Step 2: Map scalar properties from DTO to domain entity.
        ProductVariant pv = request.Adapt<ProductVariant>();
        pv.ProductId = productId;
        pv.SKU = string.IsNullOrWhiteSpace(request.SKU) ? GenerateSku() : request.SKU;

        // Step 4: Add entity to Change Tracker and persist changes.
        _context.Set<ProductVariant>().Add(pv);
        await _context.SaveChangesAsync(cancellationToken);

        // Step 5: Map persisted entity (with DB-generated ID) to response DTO.
        return pv.Adapt<ProductVariantResponse>();
    }

    public async Task<ProductVariantResponse> UpdateAsync(
        int productId,
        int id,
        ProductVariantUpdateRequest request, 
        CancellationToken cancellationToken = default
    )
    {
        // Step 1: Ensure parent product exists, this for circular dependency
        await EnsureProductExistsAsync(productId, cancellationToken);

        // Step 2: Retrieve existing variant from DB with tracking enabled
        var variant = await _context.Set<ProductVariant>()
            .FirstOrDefaultAsync(v => v.Id == id && v.ProductId == productId && !v.IsDeleted, cancellationToken)
            ?? throw new AppException($"Variant with ID {id} was not found for Product {productId}.", HttpStatusCode.NotFound);

        // Step 3: Handle SKU validation (if SKU was updated)
        // string targetSku = string.IsNullOrWhiteSpace(request.SKU) ? variant.SKU : request.SKU;

        // Step 4: Map request DTO values onto the tracked EF Core entity
        request.Adapt(variant);
        // variant.SKU = targetSku;

        // Step 5: Persist changes via EF Core Change Tracker (executes SQL UPDATE)
        await _context.SaveChangesAsync(cancellationToken);

        // Step 6: Return projected response DTO
        return variant.Adapt<ProductVariantResponse>();
    }

    public async Task<IEnumerable<ProductVariantResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<ProductVariant>()
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .ProjectToType<ProductVariantResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductVariantResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ProductVariant>()
            .AsNoTracking()
            .Where(p => p.Id == id && !p.IsDeleted)
            .ProjectToType<ProductVariantResponse>()
            .FirstOrDefaultAsync(cancellationToken) ??
            throw new AppException($"Product Variant {id} Not Exist.", HttpStatusCode.NotFound);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // 1. EF Core retrieves the entity and begins tracking it
        ProductVariant pv = await _context.Set<ProductVariant>()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken) ??
            throw new AppException($"Product Variant {id} Not Exist.", HttpStatusCode.NotFound);
        
        if (pv.IsDeleted) 
            throw new AppException($"Product Variant {id} is already Deleted.", HttpStatusCode.BadRequest);

        // 2. Memory modification: ChangeTracker detects the altered property
        pv.IsDeleted = true;

        // 3. Persistence: Generate a "UPDATE product_variants SET is_deleted = 1 WHERE id = @id"
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Validates the existence and soft-delete status of a parent <see cref="Product"/> directly from the database.
    /// </summary>
    /// <remarks>
    /// Evaluates product availability using an optimized SQL EXISTS query without loading the parent entity into memory
    /// or triggering peer service dependency resolution.
    /// </remarks>
    /// <param name="productId">The unique identifier of the parent product.</param>
    /// <exception cref="AppException">Thrown with 404 status code if the product does not exist or is soft-deleted.</exception>
    private async Task EnsureProductExistsAsync(int productId, CancellationToken cancellationToken)
    {
        bool exists = await _context.Set<Product>()
            .AnyAsync(p => p.Id == productId && !p.IsDeleted, cancellationToken);

        if (!exists)
            throw new AppException($"Product with ID {productId} was not found.", HttpStatusCode.NotFound);
    }
}
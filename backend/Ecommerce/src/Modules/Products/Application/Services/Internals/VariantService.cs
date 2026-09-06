// Ecommerce.Product.Application/Services/VariantService.cs
using System.Net;
using Mapster;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Products.Application.Common;

namespace Ecommerce.Products.Application.Services.Internals;

public class VariantService(
    AppDbContext context,
    IProductService productService) : IVariantService
{
    private readonly AppDbContext _context = context;
    private readonly IProductService _productService = productService;

    //? =====================================================================
    //?         INTERNAL HELPER METHODS
    //? =====================================================================

    // Maps a collection of variant request DTOs into domain entities attached to a parent Product.
    public List<ProductVariant> CreateVariantsFromRequests(
        List<ProductCreateVariantRequest>? variantRequests, 
        Product product)
    {
        // Step 1: Validate input collection; return an empty list if null or empty.
        if (variantRequests is null || variantRequests.Count == 0) 
            return [];

        // Step 2: Map scalar properties using Mapster
        var variants = variantRequests.Adapt<List<ProductVariant>>();

        // Step 3: Set parent reference and compute calculated domain fields
        variants.ForEach(v => 
        {
            v.Product = product;
            v.Name = ProductVariantUtils.BuildDisplayName(product.Name, v.Size, v.Color, v.DisplayColorName);
            v.NormalizedName = ProductVariantUtils.BuildNormalizedName(v.Name);
        });

        return variants;
    }
    
    //? =====================================================================
    //?         GET METHODS
    //? =====================================================================

    public async Task<IEnumerable<ProductVariantResponse>> GetVariantsByProductId(
        int productId, CancellationToken ct = default)
    {
        // check product exists
        if (!await _productService.ExistsAsync(productId, ct))
            throw new AppException($"Product with ID {productId} was not found.", HttpStatusCode.NotFound);

        // Note: Maybe in the future change responses enrich by product
        return await _context.Set<ProductVariant>()
            .AsNoTracking()
            .Where(p => p.ProductId == productId && !p.IsDeleted)
            .ProjectToType<ProductVariantResponse>()
            .ToListAsync(ct);
    }

    public async Task<ProductVariantResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Set<ProductVariant>()
            .AsNoTracking()
            .Where(p => p.Id == id && !p.IsDeleted)
            .ProjectToType<ProductVariantResponse>()
            .FirstOrDefaultAsync(ct) ??
            throw new AppException($"Product Variant {id} Not Exist.", HttpStatusCode.NotFound);
    }
    
    public async Task<IEnumerable<ProductVariantResponse>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Set<ProductVariant>()
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .ProjectToType<ProductVariantResponse>()
            .ToListAsync(ct);
    }

    //? =====================================================================
    //?         METHODS --> POST / UPDATE / DELETE 
    //? =====================================================================

    public async Task<ProductVariantResponse> CreateAsync(
        int productId,
        ProductCreateVariantRequest request, 
        CancellationToken ct = default)
    {
        // Step 1: Lightweight projection query (Only retrieves Id and Name from SQL)
        Product product = await _productService.GetEntityByIdAsync(
            productId, p => new Product { Id = p.Id, Name = p.Name }, ct);

        // Step 2: Map scalar properties from request DTO to domain entity
        ProductVariant pv = request.Adapt<ProductVariant>();
        pv.ProductId = product.Id;

        // Step 3: Compute calculated domain values
        pv.Name = ProductVariantUtils.BuildDisplayName(product.Name, pv.Size, pv.Color, pv.DisplayColorName);
        pv.NormalizedName = ProductVariantUtils.BuildNormalizedName(pv.Name);

        // Step 4: Persist entity (EF Core sets auto-generated ID)
        _context.Set<ProductVariant>().Add(pv);
        await _context.SaveChangesAsync(ct);

        // Step 5: Map persisted entity to response DTO
        return pv.Adapt<ProductVariantResponse>();
    }

    public async Task<ProductVariantResponse> UpdateAsync(
        int productId,
        int id,
        ProductVariantUpdateRequest request, 
        CancellationToken ct = default)
    {
        // Step 1: Ensure parent product exists and retrieve its master name (SQL light-query)
        var (name, slug) = await _productService.GetEntityByIdAsync(
            productId, p => Tuple.Create(p.Name, p.Slug), ct);

        // Step 2: Retrieve existing variant from DB with EF Core tracking enabled
        var pv = await _context.Set<ProductVariant>()
            .FirstOrDefaultAsync(v => v.Id == id && v.ProductId == productId && !v.IsDeleted, ct)
            ?? throw new AppException($"Variant with ID {id} was not found for Product {productId}.", HttpStatusCode.NotFound);

        /* / Step 3: Validate SKU uniqueness if changed
        if (!string.IsNullOrWhiteSpace(request.SKU) && request.SKU != pv.SKU)
        {
            var skuExists = await _context.Set<ProductVariant>()
                .AsNoTracking()
                .AnyAsync(v => v.SKU == request.SKU && v.Id != id && !v.IsDeleted, ct);

            if (skuExists)
                throw new AppException($"SKU '{request.SKU}' is already in use by another variant.", HttpStatusCode.Conflict);
        } */

        // Step 4: Map DTO values onto tracked entity
        request.Adapt(pv);

        // Step 5: Re-calculate formatted display name and search normalization token
        pv.Name = ProductVariantUtils.BuildDisplayName(name, pv.Size, pv.Color, pv.DisplayColorName);
        pv.NormalizedName = ProductVariantUtils.BuildNormalizedName(pv.Name);

        // Step 6: Persist changes via EF Core (executes SQL UPDATE for altered columns only)
        await _context.SaveChangesAsync(ct);

        // Step 7: Map persisted entity to response DTO
        return pv.Adapt<ProductVariantResponse>();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        // 1. EF Core retrieves the entity and begins tracking it
        ProductVariant pv = await _context.Set<ProductVariant>()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(ct) ??
            throw new AppException($"Product Variant {id} Not Exist.", HttpStatusCode.NotFound);
        
        if (pv.IsDeleted) 
            throw new AppException($"Product Variant {id} is already Deleted.", HttpStatusCode.BadRequest);

        // 2. Memory modification: ChangeTracker detects the altered property
        pv.IsDeleted = true;

        // 3. Persistence: Generate a "UPDATE product_variants SET is_deleted = 1 WHERE id = @id"
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
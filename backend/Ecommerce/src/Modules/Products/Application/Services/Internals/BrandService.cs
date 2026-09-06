namespace Ecommerce.Products.Application.Services.Internals;

using Mapster;
using System.Net;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Domain.Entities;

using Ecommerce.Shared.Common.Extensions;
using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;
using Ecommerce.Shared.Responses;

public class BrandService(
    AppDbContext context,
    IProductQueryService productQueryService) : IBrandService
{
    private readonly AppDbContext _context = context;
    private readonly IProductQueryService _productQueryService = productQueryService;

    public async Task<Brand> GetEntityBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await _context.Set<Brand>()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Slug == slug && !b.IsDeleted, ct)
            ?? throw new AppException(
                $"Brand with slug '{slug}' was not found.", HttpStatusCode.NotFound);
    }

    public async Task<Brand> GetEntityByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Set<Brand>()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, ct)
            ?? throw new AppException($"Brand with ID {id} was not found or is inactive.", HttpStatusCode.NotFound);
    }

    public async Task<PagedResultDto<ProductResponse>> GetProductsByBrandSlugAsync(
        string brandSlug,
        ProductFilterQuery filter,
        CancellationToken ct = default)
    {
        // Step 1: Resolve Brand Entity (Ensures 404 if slug doesn't exist)
        var brand = await GetEntityBySlugAsync(brandSlug, ct);

        // Step 2: Override filter with resolved Brand Foreign Key ID
        var brandFilter = filter with { BrandId = brand.Id };

        // Step 3: Delegate Product Query Execution by Foreign Key ID
        var pagedProducts = await _productQueryService.GetPagedProductsAsync(brandFilter, ct);

        // Step 4: Attach Brand Metadata in ExtraData for UI Header Rendering
        var extraData = new Dictionary<string, object>
        {
            ["brand"] = brand.Adapt<BrandResponse>()
        };

        return pagedProducts with { ExtraData = extraData };
    }

    //* =====================================================================
    //*         METHODS --> GET 
    //* =====================================================================

    public async Task<BrandResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        // Step 1. Query Execution: Retrieve active brand entity by ID
        // Step 2. Projection Mapping: Convert entity to lightweight response DTO
        return await _context.Set<Brand>()
            .AsNoTracking()
            .Where(b => b.Id == id)
            .ProjectToType<BrandResponse>()
            .FirstOrDefaultAsync(ct)
            ?? throw new AppException($"Brand with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<BrandDetailResponse> GetByIdDetailAsync(int id, CancellationToken ct = default)
    {
        // Step 1. Query Execution: Retrieve detailed brand entity by ID
        // Step 2. Projection Mapping: Convert entity to detailed response DTO
        return await _context.Set<Brand>()
            .AsNoTracking()
            .Where(b => b.Id == id)
            .ProjectToType<BrandDetailResponse>()
            .FirstOrDefaultAsync(ct)
            ?? throw new AppException($"Brand with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<IEnumerable<BrandResponse>> GetAllAsync(CancellationToken ct = default)
    {
        // Step 1. Query Execution & Projection: Retrieve all active brands directly mapped to DTOs
        return await _context.Set<Brand>()
            .AsNoTracking()
            .ProjectToType<BrandResponse>()
            .ToListAsync(ct);
    }

    //* =====================================================================
    //*         METHODS --> POST / PUT / DELETE 
    //* =====================================================================

    public async Task<BrandDetailResponse> CreateAsync(BrandCreateRequest request, CancellationToken ct = default)
    {
        // Step 1. Business Logic: Generate URL-friendly slug from brand name
        var slug = request.Name.ToSlug();

        // Step 2. Validation: Prevent duplicate brand entries by unique slug
        if (await _context.Set<Brand>().AnyAsync(b => b.Slug == slug, ct))
            throw new AppException($"A brand named '{request.Name}' already exists.", HttpStatusCode.BadRequest);

        // Step 3. Entity Construction: Map request DTO to domain model and set generated slug
        var brand = request.Adapt<Brand>();
        brand.Slug = slug;

        // Step 4. Persistence: Insert new entity into database context
        await _context.Set<Brand>().AddAsync(brand, ct);
        await _context.SaveChangesAsync(ct);

        // Step 5. Result Projection: Return created entity mapped to response DTO
        return brand.Adapt<BrandDetailResponse>();
    }

    public async Task<BrandDetailResponse> UpdateAsync(
        int id, 
        BrandUpdateRequest request, 
        CancellationToken ct = default)
    {
        // Step 1. Query Execution: Retrieve existing brand entity
        var brand = await _context.Set<Brand>()
            .FirstOrDefaultAsync(b => b.Id == id, ct)
            ?? throw new AppException($"Brand with ID {id} was not found.", HttpStatusCode.NotFound);

        // Step 2. Business Logic: Generate updated URL-friendly slug if Name was provided in the update DTO
        string? newSlug = request.Name?.ToSlug();

        if (newSlug is not null && newSlug != brand.Slug)
        {
            // Step 3. Validation: Prevent duplicate slug collisions with other existing brands
            if (await _context.Set<Brand>().AnyAsync(b => b.Slug == newSlug && b.Id != id, ct))
                throw new AppException($"A brand with name '{request.Name}' already exists.", HttpStatusCode.BadRequest);
        }

        // Step 4. Entity Mutation: Adapt updated values to tracked entity and update slug
        request.Adapt(brand);
        if (newSlug is not null)
            brand.Slug = newSlug;

        // Step 5. Persistence: Commit updated entity state to database
        await _context.SaveChangesAsync(ct);

        // Step 6. Result Projection: Return updated entity mapped to response DTO
        return brand.Adapt<BrandDetailResponse>();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        // Step 1. Query Execution: Retrieve target brand entity
        var brand = await _context.Set<Brand>()
            .FirstOrDefaultAsync(b => b.Id == id, ct)
            ?? throw new AppException($"Brand with ID {id} was not found.", HttpStatusCode.NotFound);

        // Step 2. Domain Validation: Prevent redundant soft deletion operations
        if (brand.IsDeleted)
            throw new AppException($"Brand with ID {id} is already deleted.", HttpStatusCode.BadRequest);

        // Step 3. Guard Clause: Prevent deletion if active products are assigned to this brand
        var hasAssociatedProducts = await _context.Set<Product>()
            .AnyAsync(p => p.BrandId == id, ct);

        if (hasAssociatedProducts)
            throw new AppException(
                $"Cannot delete brand '{brand.Name}' because it has associated products. Reassign or remove the products first.", 
                HttpStatusCode.BadRequest);

        // Step 4. Soft Delete Execution: Flag entity as logically deleted
        brand.IsDeleted = true;

        // Step 5. Persistence: Commit soft delete state to database
        await _context.SaveChangesAsync(ct);

        return true;
    }
}
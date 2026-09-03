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

public class BrandService(AppDbContext context) : IBrandService
{
    private readonly AppDbContext _context = context;

    public async Task<BrandResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // Step 1. Query Execution: Retrieve active brand entity by ID
        // Step 2. Projection Mapping: Convert entity to lightweight response DTO
        return await _context.Set<Brand>()
            .AsNoTracking()
            .Where(b => b.Id == id)
            .ProjectToType<BrandResponse>()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new AppException($"Brand with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<BrandDetailResponse> GetByIdDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        // Step 1. Query Execution: Retrieve detailed brand entity by ID
        var brand = await _context.Set<Brand>()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new AppException($"Brand with ID {id} was not found.", HttpStatusCode.NotFound);

        // Step 2. Projection Mapping: Convert entity to detailed response DTO
        return brand.Adapt<BrandDetailResponse>();
    }

    public async Task<IEnumerable<BrandResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Step 1. Query Execution & Projection: Retrieve all active brands directly mapped to DTOs
        return await _context.Set<Brand>()
            .AsNoTracking()
            .ProjectToType<BrandResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<BrandResponse> CreateAsync(BrandCreateRequest request, CancellationToken cancellationToken = default)
    {
        // Step 1. Business Logic: Generate URL-friendly slug from brand name
        var slug = request.Name.ToSlug();

        // Step 2. Validation: Prevent duplicate brand entries by unique slug
        var exists = await _context.Set<Brand>()
            .AnyAsync(b => b.Slug == slug, cancellationToken);

        if (exists)
            throw new AppException($"A brand named '{request.Name}' already exists.", HttpStatusCode.BadRequest);

        // Step 3. Entity Construction: Map request DTO to domain model and set generated slug
        var brand = request.Adapt<Brand>();
        brand.Slug = slug;

        // Step 4. Persistence: Insert new entity into database context
        await _context.Set<Brand>().AddAsync(brand, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Step 5. Result Projection: Return created entity mapped to response DTO
        return brand.Adapt<BrandResponse>();
    }

    public async Task<BrandResponse> UpdateAsync(int id, BrandUpdateRequest request, CancellationToken cancellationToken = default)
    {
        // Step 1. Query Execution: Retrieve existing brand entity
        var brand = await _context.Set<Brand>()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new AppException($"Brand with ID {id} was not found.", HttpStatusCode.NotFound);

        // Step 2. Business Logic: Generate updated URL-friendly slug from name
        var newSlug = request.Name.ToSlug();

        // Step 3. Validation: Prevent duplicate slug collisions with other existing brands
        var slugExists = await _context.Set<Brand>()
            .AnyAsync(b => b.Slug == newSlug && b.Id != id, cancellationToken);

        if (slugExists)
            throw new AppException($"A brand named '{request.Name}' already exists.", HttpStatusCode.BadRequest);

        // Step 4. Entity Mutation: Adapt updated values to tracked entity and update slug
        request.Adapt(brand);
        brand.Slug = newSlug;

        // Step 5. Persistence: Commit updated entity state to database
        await _context.SaveChangesAsync(cancellationToken);

        // Step 6. Result Projection: Return updated entity mapped to response DTO
        return brand.Adapt<BrandResponse>();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // Step 1. Query Execution: Retrieve target brand entity
        var brand = await _context.Set<Brand>()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new AppException($"Brand with ID {id} was not found.", HttpStatusCode.NotFound);

        // Step 2. Domain Validation: Prevent redundant soft deletion operations
        if (brand.IsDeleted)
            throw new AppException($"Brand with ID {id} is already deleted.", HttpStatusCode.BadRequest);

        // Step 3. Guard Clause: Prevent deletion if active products are assigned to this brand
        var hasAssociatedProducts = await _context.Set<Product>()
            .AnyAsync(p => p.BrandId == id, cancellationToken);

        if (hasAssociatedProducts)
            throw new AppException($"Cannot delete brand '{brand.Name}' because it has associated products. Reassign or remove the products first.", HttpStatusCode.BadRequest);

        // Step 4. Soft Delete Execution: Flag entity as logically deleted
        brand.IsDeleted = true;

        // Step 5. Persistence: Commit soft delete state to database
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
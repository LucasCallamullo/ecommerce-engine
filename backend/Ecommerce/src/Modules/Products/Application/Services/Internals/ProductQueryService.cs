namespace Ecommerce.Products.Application.Services.Internals;

using Mapster;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Domain.Entities;

using Ecommerce.Shared.Database;
using Ecommerce.Shared.Responses;
using Ecommerce.Shared.Common.Constants;

/// <summary>
/// Application service providing optimized read-only queries and dynamic filtering over the <see cref="Product"/> catalog.
/// </summary>
public class ProductQueryService(AppDbContext context) : IProductQueryService
{
    private readonly AppDbContext _context = context;

    public async Task<PagedResultDto<ProductResponse>> GetPagedProductsAsync(
        ProductFilterQuery filter, 
        CancellationToken ct = default)
    {
        // Step 1: Normalize & Clamp Pagination Parameters
        var (cleanPageNumber, cleanPageSize) = PaginationUtils.Normalize(filter.PageNumber, filter.PageSize);

        // Step 2: Base query for active and non-deleted products
        var query = _context.Set<Product>()
            .AsNoTracking()
            .Where(p => p.IsActive && !p.IsDeleted);

        // Step 3: Apply Hierarchy Filters (FK Direct Lookup)
        if (filter.SubcategoryId.HasValue)
        {
            query = query.Where(p => p.SubcategoryId == filter.SubcategoryId.Value);
        }
        else if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        }

        if (filter.BrandId.HasValue)
        {
            query = query.Where(p => p.BrandId == filter.BrandId.Value);
        }

        // Step 4: Apply Optional Text Search
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var cleanTerm = filter.SearchTerm.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(cleanTerm) || 
                                     p.Slug.Contains(cleanTerm));
        }

        query = filter.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Variants.Min(v => v.PriceArs)),
            "price_desc" => query.OrderByDescending(p => p.Variants.Min(v => v.PriceArs)),
            "name_asc" => query.OrderBy(p => p.Name),
            "name_desc" => query.OrderByDescending(p => p.Name),
            "oldest" => query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.Id) // Default: "newest" / created_at_desc
        };

        // Step 5: Count Total Records matching filters
        var totalCount = await query.CountAsync(ct);

        // Step 6: Apply Sorting and Pagination with Mapster SQL Projection
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((cleanPageNumber - 1) * cleanPageSize)
            .Take(cleanPageSize)
            .ProjectToType<ProductResponse>()
            .ToListAsync(ct);

        return new PagedResultDto<ProductResponse>(items, totalCount, cleanPageNumber, cleanPageSize);
    }
}
namespace Ecommerce.Products.Application.Services.Contracts;

using Ecommerce.Products.Contracts.DTOs;
using Ecommerce.Products.Contracts.Interfaces;

using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Database;
using Microsoft.EntityFrameworkCore;

public class ProductContract(AppDbContext context) : IProductContract
{
    private readonly AppDbContext _context = context;

    public async Task<ProductVariantContractDto?> GetVariantByIdAsync(int variantId, CancellationToken ct = default)
    {
        var result = await GetVariantsByIdsAsync([variantId], ct);
        return result.GetValueOrDefault(variantId);
    }

    public async Task<Dictionary<int, ProductVariantContractDto>> GetVariantsByIdsAsync(
        IEnumerable<int> variantIds,
        CancellationToken ct = default)
    {
        var ids = variantIds.Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<int, ProductVariantContractDto>();
 
        var variants = await _context.Set<ProductVariant>()
            .AsNoTracking()
            // .Include(v => v.Product)
            .Where(v => ids.Contains(v.Id) && !v.IsDeleted && !v.Product.IsDeleted)
            .Select(v => new ProductVariantContractDto(
                v.Id,                                                                 // 1
                v.Product.Slug,                                                       // 2
                v.Name,                                                               // 3
                v.MainImageUrl,                                                       // 4
                v.Size,                                                               // 5
                v.Color.HasValue ? v.Color.Value.ToString() : null,                   // 6
                v.PriceArs,                                                           // 7
                v.ComparisonPriceArs,                                                 // 8
                v.DiscountPercentageArs,                                              // 9
                v.DiscountPercentageArs > 0                                           // 10
                    ? Math.Round(v.PriceArs * (1 - (v.DiscountPercentageArs / 100m)), 2)
                    : v.PriceArs,
                v.Stock,                                                              // 11
                v.IsActive && v.Product.IsActive                                      // 12
            ))
            .ToListAsync(ct);

        return variants.ToDictionary(v => v.Id);
    }
}
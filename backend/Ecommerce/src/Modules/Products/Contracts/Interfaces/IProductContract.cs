namespace Ecommerce.Products.Contracts.Interfaces;

using Ecommerce.Products.Contracts.DTOs;

public interface IProductContract
{
    Task<ProductVariantContractDto?> GetVariantByIdAsync(int variantId, CancellationToken ct = default);

    Task<Dictionary<int, ProductVariantContractDto>> GetVariantsByIdsAsync(
        IEnumerable<int> variantIds, 
        CancellationToken ct = default
    );
}
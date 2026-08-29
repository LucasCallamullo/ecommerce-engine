// Application/Services/IVariantService.cs
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Domain.Entities;

namespace Ecommerce.Products.Application.Services;

public interface IVariantService
{
    /// <summary>
    /// Creates ProductVariant entities from request DTOs and associates them with a product.
    /// </summary>
    /// <param name="variantRequests">List of variant request DTOs</param>
    /// <param name="product">The parent product entity</param>
    /// <returns>List of created ProductVariant entities</returns>
    List<ProductVariant> CreateVariantsFromRequests(
        List<ProductCreateVariantRequest> variantRequests, 
        Product product);

    /// <summary>
    /// Generates a unique SKU for a variant.
    /// </summary>
    string GenerateSku();
}
using Mapster;
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Domain.Entities;

namespace Ecommerce.Products.Application.Mappings;

/// <summary>Configures Mapster mapping rules for <see cref="ProductVariant"/> entities.</summary>
public class VariantMappingConfig : IRegister
{
    /// <summary>Registers custom type adapter rules for product variant updates.</summary>
    /// <param name="config">The global Mapster configuration registry.</param>
    public void Register(TypeAdapterConfig config)
    {
        // Ignores null properties during variant update operations to support partial updates
        config.NewConfig<ProductVariantUpdateRequest, ProductVariant>()
            .IgnoreNullValues(true);
    }
}
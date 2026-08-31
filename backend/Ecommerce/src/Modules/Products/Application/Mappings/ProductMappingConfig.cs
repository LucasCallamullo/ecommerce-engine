using Mapster;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;

namespace Ecommerce.Products.Application.Mappings;

/// <summary> Configures Mapster mapping rules between DTOs and the <see cref="Product"/> aggregate.</summary>
public class ProductMappingConfig : IRegister
{
    /// <summary> Registers custom type adapter rules for product creation and update payloads.</summary>
    /// <param name="config">The global Mapster configuration registry.</param>
    public void Register(TypeAdapterConfig config)
    {
        // Rule 1: Ignore child variants collection on creation to prevent unmanaged graph mapping.
        config.NewConfig<ProductCreateRequest, Product>()
            .Ignore(dest => dest.Variants);

        // Rule 2: Ignore null properties on update while explicitly mapping foreign keys to allow relationship detachment.
        config.NewConfig<ProductUpdateRequest, Product>()
            .IgnoreNullValues(true)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest.SubcategoryId, src => src.SubcategoryId)
            .Map(dest => dest.BrandId, src => src.BrandId);

        // Rule 3: Map active variants to DTO list or return empty list if collection is uninitialized.
        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.Variants, src => src.Variants != null 
                ? src.Variants.Where(v => !v.IsDeleted).Adapt<List<ProductVariantResponse>>() 
                : new List<ProductVariantResponse>());
    }
}
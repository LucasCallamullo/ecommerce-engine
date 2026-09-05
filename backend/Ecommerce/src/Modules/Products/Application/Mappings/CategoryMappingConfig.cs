using Mapster;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Products.Application.DTOs.Response;

namespace Ecommerce.Products.Application.Mappings;

/// <summary>
/// Configures Mapster mapping rules between domain entities and DTOs for the <see cref="Category"/> aggregate.
/// </summary>
public class CategoryMappingConfig : IRegister
{
    /// <summary>
    /// Registers custom type adapter rules for category projection queries.
    /// </summary>
    /// <param name="config">The global Mapster configuration registry.</param>
    public void Register(TypeAdapterConfig config)
    {
        // Rule 1: Project root categories with active and non-deleted nested subcategories.
        config.NewConfig<Category, CategoryWithSubcategories>()
            .Map(dest => dest.Subcategories, src => src.Subcategories
                .Where(s => s.IsActive && !s.IsDeleted));
    }
}
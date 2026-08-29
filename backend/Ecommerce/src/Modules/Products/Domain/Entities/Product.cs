using Ecommerce.Shared.Common;

namespace Ecommerce.Products.Domain.Entities;

/// Represents a master product entity within the catalog bounded context.
/// Inherits from <see cref="BaseEntity{T}"/> to provide core auditing attributes:
// - Id (int, Primary Key) 
// - CreatedAt (DateTime, UTC timestamp upon insertion)
// - UpdatedAt (DateTime?, nullable UTC timestamp upon modification)
// - IsDeleted (bool, soft delete logical flag)
public class Product : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// Gets or sets the primary image URL for catalog listings.
    /// Denormalized field used to prevent performance bottlenecks and frequent JOIN operations against <see cref="ProductImage"/>.
    public string? MainImage { get; set; }

    /// Gets or sets a value indicating whether the product is active and visible in the store.
    public bool IsActive { get; set; } = true;

    // ====================================
    //        FK Relations
    // ====================================

    /// Gets or sets the optional foreign key referencing the primary <see cref="Category"/>.
    public int? CategoryId { get; set; }

    /// Gets or sets the navigation property for the associated primary <see cref="Category"/>.
    public Category? Category { get; set; }


    public int? SubcategoryId { get; set; }
    public Category? Subcategory { get; set; }


    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }



    /// Gets or sets the collection of physical sellable variations (SKUs) under this master product.
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    /// Gets or sets the complete gallery of images associated with this product.
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
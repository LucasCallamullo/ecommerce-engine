namespace Ecommerce.Products.Domain.Entities;

using Ecommerce.Shared.Common;

/// <summary>Represents a master product entity within the catalog bounded context.</summary>
/// <remarks>
/// Inherits from <see cref="BaseEntity{TKey}"/> to provide core auditing and soft-delete attributes:
/// <list type="bullet">
/// <item><description><c>Id</c>: Integer primary key identifier.</description></item>
/// <item><description><c>CreatedAt</c>: UTC timestamp recorded upon insertion.</description></item>
/// <item><description><c>UpdatedAt</c>: Nullable UTC timestamp recorded upon modification.</description></item>
/// <item><description><c>IsDeleted</c>: Boolean flag for logical soft deletion.</description></item>
/// </list>
/// </remarks>
public class Product : BaseEntity<int>
{
    /// <summary>
    /// Gets or sets the display name of the master product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly unique slug generated for SEO and navigational routing.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional detailed description or technical specifications of the product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the primary image URL or path for catalog listings.</summary>
    /// <remarks>
    /// Denormalized field used to prevent performance bottlenecks and frequent JOIN operations against <see cref="ProductImage"/>.
    /// </remarks>
    public string? MainImageUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is active and visible in the public store catalog.
    /// </summary>
    public bool IsActive { get; set; } = true;

    //? ====================================
    //?          FK Relations
    //? ====================================

    /// <summary>Gets or sets the optional foreign key referencing the primary <see cref="Category"/>.</summary>
    public int? CategoryId { get; set; }

    /// <summary>Gets or sets the navigation property for the associated primary <see cref="Category"/>.</summary>
    public Category? Category { get; set; }

    /// <summary>
    /// Gets or sets the optional foreign key referencing the secondary <see cref="Category"/> (subcategory).
    /// </summary>
    public int? SubcategoryId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the associated secondary <see cref="Category"/> (subcategory).
    /// </summary>
    public Category? Subcategory { get; set; }

    /// <summary>Gets or sets the optional foreign key referencing the associated <see cref="Brand"/>.</summary>
    public int? BrandId { get; set; }

    /// <summary>Gets or sets the navigation property for the associated <see cref="Brand"/>.</summary>
    public Brand? Brand { get; set; }

    /// <summary>
    /// Gets or sets the collection of physical sellable variations (SKUs) associated with this master product.
    /// </summary>
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    /// <summary>
    /// Gets or sets the complete gallery of media images associated with this product.
    /// </summary>
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
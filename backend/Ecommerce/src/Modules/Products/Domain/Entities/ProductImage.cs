namespace Ecommerce.Products.Domain.Entities;

using Ecommerce.Shared.Common;

/// <summary>
/// Represents an image asset associated with a master <see cref="Product"/> or a specific <see cref="ProductVariant"/> gallery.
/// </summary>
/// <remarks>
/// Inherits from <see cref="BaseEntity{TKey}"/> to provide standardized audit tracking 
/// (<c>Id</c>, <c>CreatedAt</c>, <c>UpdatedAt</c>, and <c>IsDeleted</c>).
/// </remarks>
public class ProductImage : BaseEntity<int>
{
    /// <summary>
    /// Gets or sets the public CDN or cloud storage URL where the image asset is hosted.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional alternative text description for accessibility (a11y) and SEO optimization.
    /// </summary>
    public string? AltText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this image is designated as the primary thumbnail or cover image.
    /// </summary>
    public bool? IsMainImage { get; set; }

    /// <summary>
    /// Gets or sets the zero-based display order index used for sorting images in gallery or carousel UI components.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the foreign key referencing the parent master product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the parent master product.
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets the optional foreign key associating this image asset directly with a specific product variant.
    /// </summary>
    public int? ProductVariantId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the specific product variant, if assigned.
    /// </summary>
    public ProductVariant? ProductVariant { get; set; }
}
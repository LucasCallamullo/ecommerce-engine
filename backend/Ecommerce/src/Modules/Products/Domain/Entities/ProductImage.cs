using Ecommerce.Shared.Common;

namespace Ecommerce.Products.Domain.Entities;

/// Represents an image asset associated with a master Product or a specific ProductVariant gallery.
/// Inherits from BaseEntity<int> to provide core auditing attributes:
// - Id (int, Primary Key)
// - CreatedAt (DateTime, UTC timestamp upon insertion)
// - UpdatedAt (DateTime?, nullable UTC timestamp upon modification)
// - IsDeleted (bool, soft delete logical flag)
public class ProductImage : BaseEntity<int>
{
    /// Public CDN or storage URL where the image asset is stored.
    public string Url { get; set; } = string.Empty;

    /// Optional alternative text description for accessibility (a11y) and SEO optimization.
    // public string? AltText { get; set; }

    /// Indicates whether this image is set as the main display image.
    public bool? MainImage { get; set; }

    /// Sort order index for displaying images in carousel or gallery UI components.
    public int DisplayOrder { get; set; }

    /// Foreign Key referencing the parent master Product.
    public int ProductId { get; set; }

    /// Navigation property to the parent master Product.
    public Product Product { get; set; } = null!;

    /// Optional Foreign Key associating this image directly to a specific ProductVariant.
    public int? ProductVariantId { get; set; }

    /// Optional Navigation property to the specific ProductVariant.
    public ProductVariant? ProductVariant { get; set; }
}
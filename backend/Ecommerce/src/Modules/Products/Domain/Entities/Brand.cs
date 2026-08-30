using Ecommerce.Shared.Common;

namespace Ecommerce.Products.Domain.Entities;

/// Represents a brand entity associated with master products in the catalog bounded context.
/// Inherits from BaseEntity<int> to provide core auditing attributes:
// - Id (int, Primary Key)
// - CreatedAt (DateTime, UTC timestamp upon insertion)
// - UpdatedAt (DateTime?, nullable UTC timestamp upon modification)
// - IsDeleted (bool, soft delete logical flag)
public class Brand : BaseEntity<int>
{
    /// Display name of the brand.
    public string Name { get; set; } = string.Empty;

    /// URL-friendly unique identifier (slug) for brand filtering.
    public string Slug { get; set; } = string.Empty;

    /// Optional logo or representative image URL for the brand.
    public string? ImageUrl { get; set; }

    /// Logical flag indicating whether the brand is active for display and filtering.
    public bool IsActive { get; set; } = true;

    /// Optional detailed description of the brand.
    public string? Description { get; set; }

    /// Collection of products associated with this brand.
    public ICollection<Product> Products { get; set; } = [];
}
namespace Ecommerce.Products.Domain.Entities;

using Ecommerce.Shared.Common;

/// <summary>
/// Represents a product brand entity within the catalog domain context.
/// </summary>
/// <remarks>
/// Inherits from <see cref="BaseEntity{TKey}"/> to provide standardized audit tracking 
/// (<c>Id</c>, <c>CreatedAt</c>, <c>UpdatedAt</c>, and <c>IsDeleted</c>).
/// </remarks>
public class Brand : BaseEntity<int>
{
    /// <summary>
    /// Gets or sets the display name of the brand.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly unique identifier (slug) used for catalog routing and brand filtering.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional absolute or relative URL pointing to the brand's logo image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the brand is currently active and visible in the public catalog.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets an optional detailed plain text or markdown description of the brand.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the navigation collection of master products associated with this brand.
    /// </summary>
    public ICollection<Product> Products { get; set; } = [];
}
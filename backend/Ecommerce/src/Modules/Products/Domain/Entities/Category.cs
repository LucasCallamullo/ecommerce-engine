namespace Ecommerce.Products.Domain.Entities;

using Ecommerce.Shared.Common;

/// <summary>
/// Represents a category entity within the product catalog domain.
/// Supports a self-referential hierarchy for root categories and nested subcategories.
/// </summary>
/// <remarks>
/// Inherits from <see cref="BaseEntity{TKey}"/> to provide standardized audit tracking 
/// (<c>Id</c>, 
/// <c>CreatedAt</c>, 
/// <c>UpdatedAt</c>, and 
/// <c>IsDeleted</c>).
/// </remarks>
public class Category : BaseEntity<int>
{
    /// <summary>
    /// Gets or sets the display name of the category.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly unique identifier (slug) used for catalog routing and filtering.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the category is active and visible in the public catalog.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets an optional detailed description explaining the scope of the category.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets an optional absolute or relative URL to a thumbnail or banner image for the category.
    /// </summary>
    public string? ImageUrl { get; set; }

    //? ====================================
    //?          FK Relations
    //? ====================================
    
    /// <summary>
    /// Gets or sets the optional foreign key referencing the parent category. 
    /// A <c>null</c> value indicates that this is a root-level category.
    /// </summary>
    public int? ParentCategoryId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the parent category.
    /// </summary>
    public Category? ParentCategory { get; set; }

    /// <summary>
    /// Gets or sets the collection of child subcategories nested directly under this node.
    /// </summary>
    public ICollection<Category> Subcategories { get; set; } = new List<Category>();

    /// <summary>
    /// Gets or sets the collection of products linked to this node as their primary parent category.
    /// </summary>
    public ICollection<Product> PrimaryProducts { get; set; } = new List<Product>();

    /// <summary>
    /// Gets or sets the collection of products linked to this node as their secondary subcategory.
    /// </summary>
    public ICollection<Product> SubcategoryProducts { get; set; } = new List<Product>();
}
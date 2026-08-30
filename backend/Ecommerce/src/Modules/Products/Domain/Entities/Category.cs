using Ecommerce.Shared.Common;

namespace Ecommerce.Products.Domain.Entities;

// Inherits from BaseEntity<int>, which automatically provides the following audited properties:
// - Id (int, Primary Key)
// - CreatedAt (DateTime, UTC timestamp upon insertion)
// - UpdatedAt (DateTime?, nullable UTC timestamp upon modification)
// - IsDeleted (bool, soft delete logical flag)

/// Represents a self-referential category hierarchy node (supports root categories and subcategories).
public class Category : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    /// Optional activity on category or subcategory
    public bool IsActive { get; set; } = true;

    /// Optional description of the category.
    public string? Description { get; set; }

    /// some brands can have urls images from brand
    public string? ImageUrl { get; set; }


    /// Foreign Key referencing the parent Category (NULL indicates a root category).
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }

    /// Collection of child subcategories under this node.
    public ICollection<Category> Subcategories { get; set; } = new List<Category>();

    /// Products assigned directly to this category as their primary category.
    public ICollection<Product> PrimaryProducts { get; set; } = new List<Product>();

    /// Products assigned to this category as their subcategory.
    public ICollection<Product> SubcategoryProducts { get; set; } = new List<Product>();
}
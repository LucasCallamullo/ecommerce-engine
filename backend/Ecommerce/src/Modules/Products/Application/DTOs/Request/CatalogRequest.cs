namespace Ecommerce.Products.Application.DTOs.Request;

// + ===========================================================
// +         Brands Requests --> Create | Update
// + ===========================================================

/// <summary>
/// Data transfer object for creating a new product brand entity in the catalog.
/// </summary>
/// <param name="Name">The unique display name of the brand (e.g., "Nike", "Apple").</param>
/// <param name="ImageUrl">Optional absolute or relative URL to the brand's logo image.</param>
/// <param name="Description">Optional detailed markdown or plain text description of the brand.</param>
/// <param name="IsActive">Logical flag indicating if the brand is available for display and filtering. Defaults to true.</param>
public record BrandCreateRequest(
    string Name,
    string? ImageUrl,
    string? Description,
    bool IsActive = true
);

/// <summary>
/// Data transfer object for updating an existing product brand entity in the catalog.
/// </summary>
/// <param name="Name">The updated unique display name of the brand.</param>
/// <param name="ImageUrl">Optional updated URL to the brand's logo image.</param>
/// <param name="Description">Optional updated detailed description of the brand.</param>
/// <param name="IsActive">Logical flag indicating whether the brand remains active for public catalog display.</param>
public record BrandUpdateRequest(
    string Name,
    string? ImageUrl,
    string? Description,
    bool IsActive
);

// + ===========================================================================
// +         Categories | Subcategories Requests --> Create | Update
// + ===========================================================================

/// <summary>
/// Represents the incoming request payload to create a new category or subcategory.
/// </summary>
/// <param name="Name">Display name of the category (e.g., "Footwear").</param>
/// <param name="Description">Optional detailed description of the category.</param>
/// <param name="ImageUrl">Optional representative image or banner URL.</param>
/// <param name="IsActive">Publishing status indicating if the category is visible in the catalog.</param>
/// <param name="ParentCategoryId">Optional foreign key referencing the parent category ID if creating a subcategory.</param>
public record CategoryCreateRequest(
    string Name,
    string? Description,
    string? ImageUrl,
    bool? IsActive,
    int? ParentCategoryId
);

/// <summary>
/// Represents the incoming request payload to update an existing category or subcategory.
/// </summary>
/// <param name="Name">Updated display name of the category.</param>
/// <param name="Description">Updated optional description of the category.</param>
/// <param name="ImageUrl">Updated optional image URL.</param>
/// <param name="IsActive">Updated publishing status for catalog visibility.</param>
/// <param name="ParentCategoryId">Updated optional parent category ID (allows re-parenting subcategories).</param>
public record CategoryUpdateRequest(
    string Name,
    string? Description,
    string? ImageUrl,
    bool? IsActive,
    int? ParentCategoryId
);
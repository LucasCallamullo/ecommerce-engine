namespace Ecommerce.Products.Application.DTOs.Response;

// + ====================================================================
// +        Category | Subcategory --> Response
// + ====================================================================

/// <summary>
/// Lightweight response payload representing essential category details for list views and nested object projections.
/// </summary>
/// <param name="Id">The unique primary key identifier of the category.</param>
/// <param name="Name">The display name of the category.</param>
/// <param name="Slug">The URL-friendly identifier used for catalog routing and filtering.</param>
/// <param name="ParentCategoryId">The optional unique identifier of the parent category, if nested.</param>
public record CategoryResponse(
    int Id,
    string Name,
    string Slug,
    int? ParentCategoryId
);

/// <summary>
/// Comprehensive data payload representing complete category metadata along with its parent category details for single-entity queries.
/// </summary>
/// <param name="Id">The unique primary key identifier of the category.</param>
/// <param name="Name">The display name of the category.</param>
/// <param name="Slug">The URL-friendly identifier used for catalog routing and filtering.</param>
/// <param name="ImageUrl">Optional absolute or relative URL pointing to the category's banner or thumbnail image.</param>
/// <param name="Description">Optional detailed description explaining the category scope.</param>
/// <param name="IsActive">Logical flag indicating whether the category is currently published and visible in the public catalog.</param>
/// <param name="ParentCategory">Optional nested lightweight representation of the immediate parent category.</param>
public record CategoryDetailResponse(
    int Id,
    string Name,
    string Slug,
    string? ImageUrl,
    string? Description,
    bool IsActive,
    CategoryResponse? ParentCategory
);

// + ====================================================================
// +        Brand Response
// + ====================================================================

/// <summary>
/// Lightweight response payload representing essential brand details for list views and nested object projections.
/// </summary>
/// <param name="Id">The unique primary key identifier of the brand.</param>
/// <param name="Name">The display name of the brand.</param>
/// <param name="Slug">The URL-friendly identifier used for catalog routing and brand filtering.</param>
public record BrandResponse(
    int Id,
    string Name,
    string Slug
);

/// <summary>
/// Comprehensive data payload representing complete brand metadata for detailed single-entity queries.
/// </summary>
/// <param name="Id">The unique primary key identifier of the brand.</param>
/// <param name="Name">The display name of the brand.</param>
/// <param name="Slug">The URL-friendly identifier used for catalog routing and brand filtering.</param>
/// <param name="ImageUrl">Optional absolute or relative URL pointing to the brand's logo image.</param>
/// <param name="Description">Optional detailed markdown or plain text description of the brand.</param>
/// <param name="IsActive">Logical flag indicating whether the brand is currently published and visible in the public catalog.</param>
public record BrandDetailResponse(
    int Id,
    string Name,
    string Slug,
    string? ImageUrl,
    string? Description,
    bool IsActive
);
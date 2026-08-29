namespace Ecommerce.Products.Application.DTOs.Response;

// Lightweight response DTO representing essential category details for nested object projections.
public record CategoryResponse(
    int Id,
    string Name,
    string Slug,
    int? ParentCategoryId
);

// Detailed data payload representing complete category metadata along with its parent category details.
public record CategoryDetailResponse(
    int Id,
    string Name,
    string Slug,
    string? ImageUrl,
    string? Description,
    bool IsActive,
    CategoryResponse? ParentCategory
);

// Lightweight response DTO representing essential brand details for nested object projections.
public record BrandResponse(
    int Id,
    string Name,
    string Slug
);

// Comprehensive data payload representing complete brand metadata for single-entity queries.
public record BrandDetailResponse(
    int Id,
    string Name,
    string Slug,
    string? ImageUrl,
    string? Description,
    bool IsActive
);
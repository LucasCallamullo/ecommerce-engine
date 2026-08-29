namespace Ecommerce.Products.Application.DTOs.Response;

/// <summary>
/// Represents the data payload for an individual product variant (SKU level).
/// </summary>
public record ProductVariantResponse(
    int Id,
    int ProductId,
    string? SKU,
    decimal PriceArs,
    decimal? ComparisonPriceArs,
    int DiscountArs,
    int Stock,
    string? Size,
    string? Color,
    string? HexColor
);

/// <summary>
/// Comprehensive data payload for a single product view, incorporating full navigational details and nested categories/brands.
/// </summary>
public record ProductDetailResponse(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string? MainImage,
    bool IsActive,
    CategoryResponse? Category,
    CategoryResponse? Subcategory,
    BrandResponse? Brand,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<ProductVariantResponse> Variants
);

/// <summary>
/// Standard data payload returned after product mutations or within catalog listing results.
/// </summary>
public record ProductResponse(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string? MainImage,
    bool IsActive,
    int? CategoryId,
    int? SubcategoryId,
    int? BrandId,
    List<ProductVariantResponse> Variants
);
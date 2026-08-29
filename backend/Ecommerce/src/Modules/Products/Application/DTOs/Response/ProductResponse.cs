namespace Ecommerce.Products.Application.DTOs.Response;

// Represents the data payload for an individual product variant (SKU level).
public record ProductVariantResponse(
    int Id,
    string? SKU,
    decimal PriceArs,
    decimal? ComparisonPriceArs,
    int DiscountArs,
    int Stock,
    string? Size,
    string? Color,
    string? HexColor
);

// Comprehensive data payload for a single product view, incorporating full navigational details.
public record ProductDetailResponse(
    int Id,
    string Name,
    string? Description,
    string? MainImage,
    bool IsActive,
    CategoryResponse? Category,
    CategoryResponse? Subcategory,
    BrandResponse? Brand,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<ProductVariantResponse> Variants
);

// Standard data payload returned after product mutations or within catalog listing results.
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
namespace Ecommerce.Products.Application.DTOs.Request;

/// <summary>
/// Payload required to update a product variant's attributes independently or partially.
/// Null properties are ignored during update operations.
/// </summary>
public record ProductVariantUpdateRequest(
    string? SKU,
    decimal? PriceArs,
    decimal? ComparisonPriceArs,
    int? DiscountArs,
    int? Stock,
    string? Size,
    string? Color,
    string? HexColor
);

/// <summary>
/// Payload required to update a master product and optionally update one of its variants.
/// Null properties indicate that the field should remain unchanged.
/// </summary>
public record ProductUpdateRequest(
    string? Name,
    string? Description,
    int? CategoryId,
    int? SubcategoryId,
    int? BrandId,
    ProductVariantUpdateRequest? Variant
);
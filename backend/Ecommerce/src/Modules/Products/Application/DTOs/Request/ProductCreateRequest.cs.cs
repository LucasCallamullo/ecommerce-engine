namespace Ecommerce.Products.Application.DTOs.Request;

// Payload required to create an initial variant alongside the master product.
public record ProductCreateVariantRequest(
    string? SKU,
    decimal PriceArs,
    decimal? ComparisonPriceArs,
    int DiscountArs,
    int Stock,
    string? Size,
    string? Color,
    string? HexColor
);

// Payload required to create a new master Product along with its initial variants.
public record ProductCreateRequest(
    string Name,
    string? Description,
    int? CategoryId,
    int? SubcategoryId,
    int? BrandId,
    bool IsActive,
    List<ProductCreateVariantRequest>? Variants = default!
)
{
    // Guarantees that if null is received from JSON deserialization, it defaults to an empty list.
    public List<ProductCreateVariantRequest> Variants { get; init; } = Variants ?? [];
}
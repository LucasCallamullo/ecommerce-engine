namespace Ecommerce.Products.Application.DTOs.Request;

/// <summary>
/// Data transfer object representing the payload required to create an initial product variant alongside a master product.
/// </summary>
/// <param name="SKU">The Stock Keeping Unit code assigned to the variant.</param>
/// <param name="PriceArs">The base selling price in Argentine Pesos (ARS).</param>
/// <param name="ComparisonPriceArs">The optional original or list price used to show discounts in ARS.</param>
/// <param name="DiscountArs">The fixed discount amount applied in ARS.</param>
/// <param name="Stock">The available physical inventory count for this variant.</param>
/// <param name="Size">The optional physical size attribute (e.g., "S", "M", "42").</param>
/// <param name="Color">The optional display name of the color attribute (e.g., "Black", "Navy Blue").</param>
/// <param name="HexColor">The optional hexadecimal color code for UI rendering (e.g., "#000000").</param>
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

/// <summary>
/// Data transfer object representing the payload required to create a new master product along with its initial variants.
/// </summary>
/// <param name="Name">The display name of the master product.</param>
/// <param name="Description">An optional detailed text description or specification of the product.</param>
/// <param name="CategoryId">The optional foreign key identifier for the primary category.</param>
/// <param name="SubcategoryId">The optional foreign key identifier for the subcategory.</param>
/// <param name="BrandId">The optional foreign key identifier for the associated brand.</param>
/// <param name="IsActive">Indicates whether the product should be published and visible immediately upon creation.</param>
/// <param name="Variants">The initial list of variants linked to this product.</param>
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
    /// <summary>
    /// Gets the list of variants for the product, defaulting to an empty list if null during deserialization.
    /// </summary>
    public List<ProductCreateVariantRequest> Variants { get; init; } = Variants ?? [];
}
namespace Ecommerce.Products.Application.DTOs.Request;

using Ecommerce.Products.Application.Common;
using Ecommerce.Shared.Common.Extensions;

/// <summary>
/// Data transfer object representing the payload required to create an initial product variant alongside a master product.
/// </summary>
/// <param name="SKU">Optional unique Stock Keeping Unit code assigned to the variant.</param>
/// <param name="PriceArs">The base selling price in Argentine Pesos (ARS).</param>
/// <param name="UnitCostArs">The internal acquisition cost price in Argentine Pesos (ARS).</param>
/// <param name="ComparisonPriceArs">Optional original or list price in ARS for strike-through UI display.</param>
/// <param name="DiscountPercentageArs">Optional discount percentage (0 to 100) applied to this variant.</param>
/// <param name="Stock">The initial available physical inventory count.</param>
/// <param name="Size">Optional physical size attribute (e.g., "S", "XL", "42").</param>
/// <param name="Color">Optional color name attribute (e.g., "Rojo", "Black").</param>
/// <param name="DisplayColorName">Optional explicit color name override to handle Spanish grammatical gender agreement in display names.</param>
/// <param name="HexColor">Optional hexadecimal color code for UI rendering (e.g., "#000000"). Resolved automatically if omitted.</param>
/// <param name="IsActive">Indicates whether the variant is active and visible in the catalog.</param>
public record ProductCreateVariantRequest(
    string? SKU,
    decimal PriceArs,
    decimal UnitCostArs,
    decimal? ComparisonPriceArs,
    int DiscountPercentageArs,
    int Stock,
    string? Size,
    string? Color,
    string? DisplayColorName,
    string? HexColor,
    bool? IsActive
)
{
    public string? SKU { get; init; } = SKU.Sanitize()?.ToUpperInvariant();

    public string? Size { get; init; } = Size.Sanitize();

    public string? Color { get; init; } = Color.Sanitize();

    public string? DisplayColorName { get; init; } = DisplayColorName.Sanitize();

    // Auto-resolves HexColor using the sanitized Color if HexColor was omitted or empty
    public string? HexColor { get; init; } = HexColor.Sanitize() 
        ?? ProductVariantUtils.ResolveHexColor(Color.Sanitize());

    public bool? IsActive { get; init; } = IsActive ?? true;
}

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
    bool? IsActive,
    List<ProductCreateVariantRequest>? Variants
)
{
    // Name is required: if sanitization returns null because it's empty or only has spaces, string.Empty is preserved.
    // so that FluentValidation correctly captures the .NotEmpty() rule.
    public string Name { get; init; } = Name.Sanitize() ?? string.Empty;

    public string? Description { get; init; } = Description.Sanitize();

    public bool? IsActive { get; init; } = IsActive ?? true;

    /// <summary>
    /// Gets the list of variants for the product, defaulting to an empty list if null or omitted during deserialization.
    /// </summary>
    public List<ProductCreateVariantRequest> Variants { get; init; } = Variants ?? [];
}
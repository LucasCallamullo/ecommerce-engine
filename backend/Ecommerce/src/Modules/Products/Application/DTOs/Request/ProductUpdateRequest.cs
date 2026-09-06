namespace Ecommerce.Products.Application.DTOs.Request;

using Ecommerce.Products.Application.Common;
using Ecommerce.Shared.Common.Extensions;

/// <summary>
/// Data transfer object representing the payload required to update a product variant's attributes independently or partially.
/// </summary>
/// <remarks>
/// All properties are nullable. Properties left as <c>null</c> indicate that their current state should remain unchanged during update operations.
/// </remarks>
/// <param name="SKU">The optional Stock Keeping Unit code to update.</param>
/// <param name="PriceArs">The optional base selling price in Argentine Pesos (ARS).</param>
/// <param name="UnitCostArs">The optional base cost price in Argentine Pesos (ARS).</param>
/// <param name="ComparisonPriceArs">The optional list or original price in ARS used to display discounts.</param>
/// <param name="DiscountPercentageArs">The optional discount percentage applied (0 to 100).</param>
/// <param name="Stock">The optional available physical inventory count.</param>
/// <param name="Size">The optional physical size attribute (e.g., "S", "M", "42").</param>
/// <param name="Color">The optional display name of the color attribute (e.g., "Negro", "Azul").</param>
/// <param name="DisplayColorName">The optional explicit color name override for Spanish grammatical gender agreement.</param>
/// <param name="HexColor">The optional hexadecimal color code for UI rendering (e.g., "#000000").</param>
/// <param name="IsActive">The optional publishing visibility status.</param>
public record ProductVariantUpdateRequest(
    string? SKU,
    decimal? PriceArs,
    decimal? UnitCostArs,
    decimal? ComparisonPriceArs,
    int? DiscountPercentageArs,
    int? Stock,
    string? Size,
    string? Color,
    string? DisplayColorName,
    string? HexColor,
    bool? IsActive
)
{
    public string? SKU { get; init; } = SKU.Sanitize()?.ToUpperInvariant() ?? ProductVariantUtils.GenerateSku();

    public string? Size { get; init; } = Size.Sanitize();

    public string? Color { get; init; } = Color.Sanitize();

    public string? DisplayColorName { get; init; } = DisplayColorName.Sanitize();

    // Auto-resolves HexColor using the sanitized Color if HexColor was omitted or empty
    public string? HexColor { get; init; } = HexColor.Sanitize() 
        ?? ProductVariantUtils.ResolveHexColor(Color.Sanitize());

    public bool? IsActive { get; init; } = IsActive ?? true;
}

/// <summary>
/// Data transfer object representing the payload required to update a master product's information.
/// </summary>
/// <remarks>
/// Properties left as <c>null</c> indicate that the corresponding field should remain unchanged in the system.
/// </remarks>
/// <param name="Name">The optional new display name of the master product.</param>
/// <param name="Description">The optional new detailed text description of the product.</param>
/// <param name="CategoryId">The optional primary category foreign key identifier.</param>
/// <param name="SubcategoryId">The optional subcategory foreign key identifier.</param>
/// <param name="BrandId">The optional associated brand foreign key identifier.</param>
/// <param name="MainImage">The optional relative path or URL of the product's primary display image.</param>
/// <param name="IsActive">The optional publishing visibility status of the master product.</param>
public record ProductUpdateRequest(
    string? Name,
    string? Description,
    int? CategoryId,
    int? SubcategoryId,
    int? BrandId,
    string? MainImage,
    bool? IsActive
)
{
    public string? Name { get; init; } = Name.Sanitize();

    public string? Description { get; init; } = Description.Sanitize();

    public string? MainImage { get; init; } = MainImage.Sanitize();

    public bool? IsActive { get; init; } = IsActive ?? true;
}
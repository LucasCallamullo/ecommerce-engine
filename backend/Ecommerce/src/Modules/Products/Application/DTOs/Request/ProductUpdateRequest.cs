namespace Ecommerce.Products.Application.DTOs.Request;

/// <summary>
/// Data transfer object representing the payload required to update a product variant's attributes independently or partially.
/// </summary>
/// <remarks>
/// All properties are nullable. Properties left as <c>null</c> indicate that their current state should remain unchanged during update operations.
/// </remarks>
/// <param name="SKU">The optional Stock Keeping Unit code to update.</param>
/// <param name="PriceArs">The optional base selling price in Argentine Pesos (ARS).</param>
/// <param name="ComparisonPriceArs">The optional list or original price in ARS used to display discounts.</param>
/// <param name="DiscountArs">The optional fixed discount amount applied in ARS.</param>
/// <param name="Stock">The optional available physical inventory count.</param>
/// <param name="Size">The optional physical size attribute (e.g., "S", "M", "42").</param>
/// <param name="Color">The optional display name of the color attribute.</param>
/// <param name="HexColor">The optional hexadecimal color code for UI rendering (e.g., "#000000").</param>
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
/// <param name="IsActive">Indicates whether the product visibility remains active. Defaults to <c>true</c>.</param>
public record ProductUpdateRequest(
    string? Name,
    string? Description,
    int? CategoryId,
    int? SubcategoryId,
    int? BrandId,
    string? MainImage,
    bool IsActive = true
    // ProductVariantUpdateRequest? Variant
);
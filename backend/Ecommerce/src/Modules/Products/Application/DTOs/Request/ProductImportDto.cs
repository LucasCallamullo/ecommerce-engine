namespace Ecommerce.Products.Application.DTOs.Request;

/// <summary>
/// Domain-neutral record representing an unvalidated product row extracted directly from an Excel spreadsheet.
/// Serves as the primary output contract for <see cref="IProductExcelParser"/> before business validation and persistence.
/// </summary>
/// <param name="Name">Raw display name of the master product.</param>
/// <param name="GroupProduct">Optional group code (e.g., 'same_variant') used to group multiple rows under a single master product.</param>
/// <param name="Size">Optional size attribute (e.g., 'XL', 'L', '42') appended to the variant display name.</param>
/// <param name="Color">Optional raw color attribute or hex-resolvable color name (e.g., 'Black', 'Rojo').</param>
/// <param name="ColorName">Optional explicit color name override to handle Spanish grammatical gender agreement in variant display names (e.g., 'Blanca').</param>
/// <param name="PriceArs">Base retail selling price in Argentine Pesos (ARS).</param>
/// <param name="UnitCostArs">Optional internal supplier acquisition cost in ARS used for profit margin calculation.</param>
/// <param name="IsActive">Initial catalog publishing visibility status indicating whether the product is enabled in the public store.</param>
/// <param name="Stock">Initial inventory stock quantity available for this product variant.</param>
/// <param name="Category">Optional target root category name used for database lookup or automatic creation.</param>
/// <param name="Subcategory">Optional secondary child category name linked to the root parent category.</param>
/// <param name="Brand">Optional manufacturer or brand name associated with the product.</param>
/// <param name="DiscountPercentageArs">Optional percentage discount (0 to 100) applied to this product variant.</param>
/// <param name="Description">Optional detailed product narrative or marketing description.</param>
/// <param name="ImageUrl">Primary media URL representing the main product thumbnail.</param>
/// <param name="ImageUrl2">Optional secondary media URL for gallery representation.</param>
public record ProductImportDto(
    string Name,
    string? GroupProduct,    // optional to create variants on import
    string? Size,            // optional to adapt with Name Variant eg xl l, 40 42
    string? Color,            // optional to adapt with Name Variant 
    string? ColorName,        // optional to adapt with Name Variant, override Color on Name
    decimal? PriceArs,        // safe for parser excel but must has a value
    decimal? UnitCostArs,        // safe for parser excel but must has a value
    bool IsActive,        // reference to Available or IsActive
    int? Stock,            // safe for parser excel but must has a value
    string? Category,
    string? Subcategory,
    string? Brand,
    int? DiscountPercentageArs,
    string? Description,
    string? ImageUrl,
    string? ImageUrl2
);
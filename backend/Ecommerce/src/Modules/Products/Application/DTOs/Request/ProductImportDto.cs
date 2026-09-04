namespace Ecommerce.Products.Application.DTOs.Request;

/// <summary>
/// Domain-neutral record representing an unvalidated product row extracted directly from an Excel spreadsheet.
/// Serves as the primary output contract for <see cref="IProductExcelParser"/> before business validation and persistence.
/// </summary>
/// <param name="Name">Raw display name of the master product.</param>
/// <param name="PriceArs">Base retail price in Argentine Pesos (ARS).</param>
/// <param name="Available">Initial catalog publishing visibility status.</param>
/// <param name="Stock">Initial inventory stock quantity for the default product variant.</param>
/// <param name="Category">Optional Target root category name used for database lookup or creation.</param>
/// <param name="Subcategory">Optional secondary child category name linked to the root parent.</param>
/// <param name="Brand">Optional manufacturer or brand name associated with the product.</param>
/// <param name="DiscountArs">Optional flat discount amount in ARS to apply to the base price.</param>
/// <param name="Description">Optional detailed product narrative or marketing description.</param>
/// <param name="ImageUrl">Primary media URL representing the main product thumbnail.</param>
/// <param name="ImageUrl2">Optional secondary media URL for gallery representation.</param>
public record ProductImportDto(
    string Name,
    decimal? PriceArs,        // safe for parser excel but must has a value
    bool Available,
    int? Stock,            // safe for parser excel but must has a value
    string? Category,
    string? Subcategory,
    string? Brand,
    int? DiscountArs,
    string? Description,
    string? ImageUrl,
    string? ImageUrl2
);
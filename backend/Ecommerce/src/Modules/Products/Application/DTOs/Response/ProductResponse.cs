namespace Ecommerce.Products.Application.DTOs.Response;

/// <summary>
/// Data transfer object representing the payload for an individual product variant at the Stock Keeping Unit (SKU) level.
/// </summary>
/// <param name="Id">The unique identifier for the product variant.</param>
/// <param name="ProductId">The foreign key identifier linking this variant to its parent master product.</param>
/// <param name="SKU">The optional Stock Keeping Unit code assigned to this variant.</param>
/// <param name="PriceArs">The current selling price in Argentine Pesos (ARS).</param>
/// <param name="ComparisonPriceArs">The optional list or original price in ARS used to display price reductions.</param>
/// <param name="DiscountArs">The fixed discount amount applied to this variant in ARS.</param>
/// <param name="Stock">The available physical inventory count for this variant.</param>
/// <param name="Size">The physical size attribute associated with this variant (e.g., "S", "M", "42").</param>
/// <param name="Color">The display name of the color attribute (e.g., "Black", "Navy Blue").</param>
/// <param name="HexColor">The hexadecimal color code used for UI swatch rendering (e.g., "#000000").</param>
public record ProductVariantResponse(
    int Id,
    int ProductId,
    string Name,
    bool IsActive,
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
/// Comprehensive data transfer object for a detailed single product view, incorporating full navigational metadata and nested relation payloads.
/// </summary>
/// <param name="Id">The unique primary key identifier of the master product.</param>
/// <param name="Name">The display name of the product.</param>
/// <param name="Slug">The URL-friendly unique slug generated from the product name.</param>
/// <param name="Description">The detailed text description or specifications of the product.</param>
/// <param name="MainImage">The relative path or URL pointing to the primary product display image.</param>
/// <param name="IsActive">Indicates whether the product is published and visible to end users.</param>
/// <param name="Category">The full nested primary category payload details.</param>
/// <param name="Subcategory">The full nested subcategory payload details.</param>
/// <param name="Brand">The full nested brand payload details.</param>
/// <param name="CreatedAt">The UTC timestamp indicating when the product record was created.</param>
/// <param name="UpdatedAt">The UTC timestamp indicating when the product record was last modified.</param>
/// <param name="Variants">The collection of variants linked to this master product.</param>
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
/// Standard data transfer object returned following product mutations or within catalog listing results.
/// </summary>
/// <param name="Id">The unique primary key identifier of the master product.</param>
/// <param name="Name">The display name of the product.</param>
/// <param name="Slug">The URL-friendly unique slug generated from the product name.</param>
/// <param name="Description">The detailed text description of the product.</param>
/// <param name="MainImage">The relative path or URL pointing to the primary display image.</param>
/// <param name="IsActive">Indicates whether the product is currently active in the catalog.</param>
/// <param name="CategoryId">The optional foreign key identifier of the primary category.</param>
/// <param name="SubcategoryId">The optional foreign key identifier of the subcategory.</param>
/// <param name="BrandId">The optional foreign key identifier of the associated brand.</param>
/// <param name="Variants">The collection of variants associated with this product.</param>
public record ProductResponse(
    int Id,
    // string Name,
    string Slug,
    string? Description,
    string? MainImage,
    bool IsActive,
    int? CategoryId,
    int? SubcategoryId,
    int? BrandId,
    List<ProductVariantResponse> Variants
);
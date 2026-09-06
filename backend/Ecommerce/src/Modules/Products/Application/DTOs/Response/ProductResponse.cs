using Ecommerce.Products.Domain.Enums;

namespace Ecommerce.Products.Application.DTOs.Response;

/// <summary>
/// Comprehensive data transfer object providing complete state information for a single product variant, typically returned in backoffice administration views or detailed SKU audits.
/// </summary>
/// <param name="Id">The unique primary key identifier of the product variant.</param>
/// <param name="ProductId">The foreign key identifier linking this variant to its parent master product.</param>
/// <param name="Name">The formatted, user-facing composite display name of the variant.</param>
/// <param name="MainImageUrl">The relative path or URL pointing to the primary display image assigned to this variant.</param>
/// <param name="PriceArs">The current base selling price in Argentine Pesos (ARS).</param>
/// <param name="UnitCostArs">The unit cost price in ARS used for internal profit margin calculations.</param>
/// <param name="ComparisonPriceArs">The optional list or original reference price in ARS used to show strike-through price reductions.</param>
/// <param name="DiscountArs">The calculated fixed discount amount applied to this variant in ARS.</param>
/// <param name="IsActive">Indicates whether this specific variant is active and available for customer purchases.</param>
/// <param name="SKU">The optional Stock Keeping Unit code assigned to this variant.</param>
/// <param name="Stock">The physical inventory count available for this variant.</param>
/// <param name="Size">The physical size attribute value associated with this variant (e.g., "S", "M", "42").</param>
/// <param name="Color">The normalized master catalog base color enum value.</param>
/// <param name="DisplayColorName">The optional custom or commercial color display name override.</param>
/// <param name="HexColor">The hexadecimal color code used for visual swatch rendering in the frontend UI (e.g., "#000000").</param>
/// <param name="CreatedAt">The UTC timestamp indicating when the variant record was created.</param>
/// <param name="UpdatedAt">The optional UTC timestamp indicating when the variant record was last modified.</param>
public record VariantDetailResponse(
    int Id,
    int ProductId,
    string Name,
    // string NormalizedName,
    string? MainImageUrl,
    
    decimal PriceArs,
    decimal UnitCostArs,
    decimal? ComparisonPriceArs,
    int DiscountArs,

    bool IsActive,
    string? SKU,
    int Stock,
    string? Size,
    ColorEnum? Color,
    string? DisplayColorName,
    string? HexColor,

    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Lightweight data transfer object representing essential product variant details embedded within catalog listings or master product views.
/// </summary>
/// <param name="Id">The unique primary key identifier of the product variant.</param>
/// <param name="ProductId">The foreign key identifier linking this variant to its parent master product.</param>
/// <param name="Name">The formatted, user-facing composite display name of the variant.</param>
/// <param name="MainImageUrl">The relative path or URL pointing to the primary display image assigned to this variant.</param>
/// <param name="DiscountArs">The calculated fixed discount amount applied to this variant in ARS.</param>
/// <param name="PriceArs">The current base selling price in Argentine Pesos (ARS).</param>
/// <param name="ComparisonPriceArs">The optional list or original reference price in ARS used to show strike-through price reductions.</param>
/// <param name="Stock">The physical inventory count available for this variant.</param>
/// <param name="IsActive">Indicates whether this specific variant is active and available for customer purchases.</param>
/// <param name="Size">The physical size attribute value associated with this variant (e.g., "S", "M", "42").</param>
/// <param name="Color">The normalized master catalog base color enum value.</param>
/// <param name="HexColor">The hexadecimal color code used for visual swatch rendering in the frontend UI (e.g., "#000000").</param>
/// <param name="CreatedAt">The UTC timestamp indicating when the variant record was created.</param>
/// <param name="UpdatedAt">The optional UTC timestamp indicating when the variant record was last modified.</param>
public record VariantResponse(
    int Id,
    int ProductId,
    string Name,
    string? MainImageUrl,
    
    int DiscountArs,
    decimal PriceArs,
    decimal? ComparisonPriceArs,
    int Stock,
    bool IsActive,

    // string? SKU,
    string? Size,
    ColorEnum? Color,
    string? HexColor,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Standard data transfer object returned following master product mutations or within condensed catalog search results.
/// </summary>
/// <param name="Id">The unique primary key identifier of the master product.</param>
/// <param name="Slug">The URL-friendly unique slug generated from the product name.</param>
/// <param name="MainImageUrl">The relative path or URL pointing to the primary display image of the product.</param>
/// <param name="CategoryId">The optional foreign key identifier of the primary category.</param>
/// <param name="SubcategoryId">The optional foreign key identifier of the subcategory.</param>
/// <param name="BrandId">The optional foreign key identifier of the associated brand.</param>
/// <param name="Variants">The collection of simplified variants linked to this master product.</param>
public record ProductResponse(
    int Id,
    string Slug,
    string? MainImageUrl,
    int? CategoryId,
    int? SubcategoryId,
    int? BrandId,
    List<VariantResponse> Variants
);

/// <summary>
/// Comprehensive data transfer object for a detailed single product view, incorporating full navigational metadata and nested relation payloads.
/// </summary>
/// <param name="Id">The unique primary key identifier of the master product.</param>
/// <param name="Name">The display name of the master product.</param>
/// <param name="Slug">The URL-friendly unique slug generated from the product name.</param>
/// <param name="Description">The detailed text description or specifications of the product.</param>
/// <param name="IsActive">Indicates whether the product is published and visible to end users.</param>
/// <param name="Category">The full nested primary category payload details.</param>
/// <param name="Subcategory">The full nested subcategory payload details.</param>
/// <param name="Brand">The full nested brand payload details.</param>
/// <param name="Variants">The collection of variants linked to this master product.</param>
public record ProductDetailResponse(
    int Id,
    string Name,
    string Slug,
    string? Description,
    // string? MainImage,
    bool IsActive,
    CategoryResponse? Category,
    CategoryResponse? Subcategory,
    BrandResponse? Brand,
    // DateTime CreatedAt,
    // DateTime? UpdatedAt,
    List<VariantResponse> Variants
);
namespace Ecommerce.Products.Application.Request.DTOs;

// + ===========================================================================
// +         Product Image Requests --> Create | Update
// + ===========================================================================

/// <summary>
/// Represents the incoming request payload to attach a new image asset to a product or variant.
/// </summary>
/// <param name="Url">Public CDN or storage URL where the image asset is hosted.</param>
/// <param name="AltText">Optional alternative text description for accessibility (a11y) and SEO.</param>
/// <param name="MainImage">Optional flag indicating if this image should be set as the main display image.</param>
/// <param name="DisplayOrder">Sort order index for gallery or carousel display.</param>
/// <param name="ProductId">Foreign key referencing the parent master product.</param>
/// <param name="ProductVariantId">Optional foreign key associating the image directly with a specific variant.</param>
public record ProductImageCreateRequest(
    string Url,
    string? AltText,
    bool? MainImage,
    int? DisplayOrder,
    int ProductId,
    int? ProductVariantId
);

/// <summary>
/// Represents the incoming request payload to update an existing image asset's metadata or placement.
/// </summary>
/// <param name="Url">Updated public CDN or storage URL where the image asset is hosted.</param>
/// <param name="AltText">Updated alternative text description for accessibility and SEO.</param>
/// <param name="MainImage">Updated flag indicating if this image should be set as the main display image.</param>
/// <param name="DisplayOrder">Updated sort order index for gallery positioning.</param>
/// <param name="ProductVariantId">Updated optional foreign key linking the image to a variant.</param>
public record ProductImageUpdateRequest(
    string Url,
    string? AltText,
    bool? MainImage,
    int? DisplayOrder,
    int? ProductVariantId
);
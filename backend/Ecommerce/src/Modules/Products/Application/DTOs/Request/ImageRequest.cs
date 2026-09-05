namespace Ecommerce.Products.Application.Request.DTOs;

using Ecommerce.Shared.Common.Extensions;

// + ===========================================================================
// +         Product Image Requests --> Create | Update
// + ===========================================================================

/// <summary>
/// Represents the incoming request payload to attach a new image asset to a product or variant.
/// </summary>
/// <param name="Url">Public CDN or storage URL where the image asset is hosted.</param>
/// <param name="AltText">Optional alternative text description for accessibility (a11y) and SEO.</param>
/// <param name="MainImage">Optional flag indicating if this image should be set as the main display image. Defaults to false.</param>
/// <param name="DisplayOrder">Optional sort order index for gallery or carousel display. Defaults to 0.</param>
/// <param name="ProductId">Foreign key referencing the parent master product.</param>
/// <param name="ProductVariantId">Optional foreign key associating the image directly with a specific variant.</param>
public record ProductImageCreateRequest(
    string Url,
    string? AltText,
    bool? MainImage,
    int? DisplayOrder,
    int ProductId,
    int? ProductVariantId
)
{
    // Url es requerida: si viniera compuesta por puros espacios o HTML, .Sanitize() la normaliza
    // a string.Empty para que FluentValidation dispare la regla .NotEmpty() limpia.
    public string Url { get; init; } = Url.Sanitize() ?? string.Empty;

    public string? AltText { get; init; } = AltText.Sanitize();

    public bool? MainImage { get; init; } = MainImage ?? false;

    public int? DisplayOrder { get; init; } = DisplayOrder ?? 0;
}

/// <summary>
/// Represents the incoming request payload to update an existing image asset's metadata or placement.
/// </summary>
/// <param name="Url">Optional updated public CDN or storage URL where the image asset is hosted.</param>
/// <param name="AltText">Optional updated alternative text description for accessibility and SEO.</param>
/// <param name="MainImage">Optional updated flag indicating if this image should be set as the main display image.</param>
/// <param name="DisplayOrder">Optional updated sort order index for gallery positioning.</param>
/// <param name="ProductVariantId">Optional updated foreign key linking the image to a variant.</param>
public record ProductImageUpdateRequest(
    string? Url,
    string? AltText,
    bool? MainImage,
    int? DisplayOrder,
    int? ProductVariantId
)
{
    public string? Url { get; init; } = Url.Sanitize();

    public string? AltText { get; init; } = AltText.Sanitize();
}
namespace Ecommerce.Products.Application.DTOs.Request;

/// <summary>
/// Data transfer object for creating a new product brand entity in the catalog.
/// </summary>
/// <param name="Name">The unique display name of the brand (e.g., "Nike", "Apple").</param>
/// <param name="ImageUrl">Optional absolute or relative URL to the brand's logo image.</param>
/// <param name="Description">Optional detailed markdown or plain text description of the brand.</param>
/// <param name="IsActive">Logical flag indicating if the brand is available for display and filtering. Defaults to true.</param>
public record BrandCreateRequest(
    string Name,
    string? ImageUrl,
    string? Description,
    bool IsActive = true
);

/// <summary>
/// Data transfer object for updating an existing product brand entity in the catalog.
/// </summary>
/// <param name="Name">The updated unique display name of the brand.</param>
/// <param name="ImageUrl">Optional updated URL to the brand's logo image.</param>
/// <param name="Description">Optional updated detailed description of the brand.</param>
/// <param name="IsActive">Logical flag indicating whether the brand remains active for public catalog display.</param>
public record BrandUpdateRequest(
    string Name,
    string? ImageUrl,
    string? Description,
    bool IsActive
);
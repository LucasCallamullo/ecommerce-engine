namespace Ecommerce.Shared.Common.Validations;

/// <summary>
/// Provides centralized validation helper algorithms for FluentValidation rules across application modules.
/// </summary>
public static class ValidationHelpers
{
    /// <summary>
    /// Validates whether a string is a well-formed absolute URL matching accepted URI schemes (e.g., HTTP, HTTPS, S3).
    /// </summary>
    /// <param name="url">The string representation of the URL to validate.</param>
    /// <returns>
    /// <c>true</c> if the string is a non-empty, well-formed absolute URL matching an allowed scheme; otherwise, <c>false</c>.
    /// </returns>
    public static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
            return false;

        // Centralized scheme validation logic
        return uriResult.Scheme == Uri.UriSchemeHttp 
            || uriResult.Scheme == Uri.UriSchemeHttps
            || uriResult.Scheme == "s3";
    }

    /// <summary>
    /// Validates whether a string is a well-formed absolute URL that points to a supported image format.
    /// </summary>
    /// <param name="url">The string representation of the image URL to validate.</param>
    /// <returns>
    /// <c>true</c> if the URL is valid and ends with an allowed image file extension 
    /// (.png, .jpg, .jpeg, .webp, .svg); otherwise, <c>false</c>.
    /// </returns>
    public static bool BeAValidImageUrl(string? url)
    {
        if (!BeAValidUrl(url)) 
            return false;

        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".webp", ".svg" };
        
        return allowedExtensions.Any(ext => url!.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}
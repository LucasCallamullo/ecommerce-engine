namespace Ecommerce.Shared.Common.Extensions;

using System.Text.RegularExpressions;

public static partial class StringExtensions
{
    /// <summary>
    /// Trims whitespace, replaces null or whitespace-only inputs with null, 
    /// and strips raw HTML tags (<...>) to prevent basic XSS persistence.
    /// </summary>
    public static string? Sanitize(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        // Step 1: Trim leading and trailing whitespace
        var sanitized = text.Trim();

        // Step 2: Strip HTML tags to ensure safe string persistence
        sanitized = Regex.Replace(sanitized, @"<[^>]*>", string.Empty);

        // Step 3: Normalize internal multiple spaces into single spaces
        sanitized = Regex.Replace(sanitized, @"\s+", " ");

        return string.IsNullOrWhiteSpace(sanitized) ? null : sanitized;
    }

    /// <summary>
    /// Converts a string into a URL-friendly slug.
    /// Handles Spanish diacritics, removes non-alphanumeric characters, and converts spaces to hyphens.
    /// </summary>
    public static string ToSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Step 1: Normalize case and trim whitespace
        var slug = text.ToLowerInvariant().Trim();

        // Step 2: Replace Spanish accents/diacritics and special characters
        slug = slug.Replace("á", "a")
                   .Replace("é", "e")
                   .Replace("í", "i")
                   .Replace("ó", "o")
                   .Replace("ú", "u")
                   .Replace("ü", "u")
                   .Replace("ñ", "n");

        // Step 3: Remove invalid non-alphanumeric characters
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Step 4: Convert multiple spaces or hyphens into a single hyphen
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

        return slug;
    }
}
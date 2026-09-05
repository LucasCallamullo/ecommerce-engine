namespace Ecommerce.Shared.Common.Extensions;

using System.Text.RegularExpressions;

public static partial class StringExtensions
{
    /// <summary>
    /// Trims whitespace, replaces null or whitespace-only inputs with null, 
    /// strips raw HTML tags to prevent basic XSS, and collapses internal spaces.
    /// </summary>
    public static string? Sanitize(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        // Step 1: Trim leading and trailing whitespace
        var sanitized = text.Trim();

        // Step 2: Strip HTML tags to ensure safe string persistence
        sanitized = HtmlTagRegex().Replace(sanitized, string.Empty);

        // Step 3: Normalize internal multiple spaces into single spaces
        sanitized = MultipleSpacesRegex().Replace(sanitized, " ");

        return string.IsNullOrWhiteSpace(sanitized) ? null : sanitized;
    }

    /// <summary>
    /// Converts a string into a URL-friendly slug.
    /// Handles Spanish diacritics, removes non-alphanumeric characters, and converts spaces to hyphens.
    /// </summary>
    public static string ToSlug(this string text)
    {
        // Step 1: Return empty string immediately if the input is null or whitespace
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Step 2: Normalize case to lower invariant and trim external whitespace
        var slug = text.ToLowerInvariant().Trim();

        // Step 3: Replace Spanish specific diacritics and special characters
        slug = slug.Replace("á", "a")
                   .Replace("é", "e")
                   .Replace("í", "i")
                   .Replace("ó", "o")
                   .Replace("ú", "u")
                   .Replace("ü", "u")
                   .Replace("ñ", "n");

        // Step 4: Remove any non-alphanumeric characters except whitespace and hyphens
        slug = InvalidSlugCharsRegex().Replace(slug, string.Empty);

        // Step 5: Convert multiple spaces into a single space
        slug = MultipleSpacesRegex().Replace(slug, " ");

        // Step 6: Convert single spaces or multiple hyphens into a single hyphen and trim edges
        slug = SlugHyphensRegex().Replace(slug, "-").Trim('-');

        return slug;
    }

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex InvalidSlugCharsRegex();

    [GeneratedRegex(@"[\s-]+")]
    private static partial Regex SlugHyphensRegex();

    [GeneratedRegex(@"<[^>]*>")]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleSpacesRegex();
}
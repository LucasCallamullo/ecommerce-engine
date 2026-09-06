namespace Ecommerce.Products.Application.Common;

using System.Globalization;
using System.Text;

/// <summary>
/// Utility methods for building standardized display names and search-normalized 
/// strings for product variants across the catalog module.
/// </summary>
public static class ProductVariantUtils
{
    /// <summary>
    /// Builds the user-facing display name for a product variant using normalized casing conventions.
    /// Format rules: Colors are Title Cased (e.g., "negro" -> "Negro"), and text sizes are UPPERCASE (e.g., "xl" -> "XL").
    /// </summary>
    /// <param name="productName">The master product display name.</param>
    /// <param name="size">The optional size attribute value (e.g., "m", "42").</param>
    /// <param name="color">The optional base color name attribute.</param>
    /// <param name="colorName">The optional explicit color display name override for gender or grammatical agreement.</param>
    /// <returns>A formatted composite display name (e.g., "Remera Oversize - Negra - XL").</returns>
    public static string BuildDisplayName(
        string productName, string? size, string? color, string? colorName)
    {
        // Step 1: Resolve and format color attribute (colorName override takes precedence over base color)
        var selectedColor = !string.IsNullOrWhiteSpace(colorName) ? colorName : color;
        var formattedColor = FormatColor(selectedColor);

        // Step 2: Apply specific casing rules to the size attribute
        var formattedSize = FormatSize(size);

        // Step 3: Filter out null or empty components and combine with single space separator
        var parts = new[] { productName, formattedSize, formattedColor }
            .Where(p => !string.IsNullOrWhiteSpace(p));

        return string.Join(" ", parts);
    }

    /// <summary>
    /// Builds an accent-free, lowercase normalized search term for variant indexing.
    /// This method MUST be called whenever the variant's attributes or display name change to keep search tokens strictly synchronized in the database.
    /// </summary>
    /// <param name="productName">The master product display name.</param>
    /// <param name="size">The optional size attribute value.</param>
    /// <param name="color">The optional base color name attribute.</param>
    /// <param name="colorName">The optional explicit color display name override.</param>
    /// <returns>A clean, lowercased, diacritic-free search token (e.g., "remera oversize negra xl").</returns>
    public static string BuildNormalizedName(string productName, string? size, string? color, string? colorName)
    {
        // Step 1: Generate the authoritative formatted display name first
        var displayName = BuildDisplayName(productName, size, color, colorName);

        // Step 2: Strip accents, remove special characters, and lowercase for database/search indexing
        return displayName.ToSearchNormalized();
    }

    public static string BuildNormalizedName(string productNameFormatted)
    {
        // Step 1: Strip accents, remove special characters, and lowercase for database/search indexing
        return productNameFormatted.ToSearchNormalized();
    }

    /// <summary> Helper method to generate a fallback unique SKU identifier. </summary>
    public static string GenerateSku()
    {
        // Step 1: Generate a pseudo-random 4-digit numerical suffix using thread-safe Random.Shared.
        var random = Random.Shared.Next(1, 9999).ToString();
        var random2 = Random.Shared.Next(1, 9999).ToString();
        return $"SKU-{random}-{random2}";
    }

    /// <summary>
    /// Normalizes text for fast natural language searches (removes diacritics/accents and converts to lower case).
    /// </summary>
    public static string ToSearchNormalized(this string input)
    {
        // Step 1: Return empty string immediately if the input is null or whitespace
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Step 2: Separate characters and their diacritical marks (Unicode Form D)
        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        // Step 3: Filter out diacritics (accents, tildes, non-spacing marks)
        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        // Step 4: Re-compose string (Form C), convert to lowercase, and trim trailing whitespace
        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant()
            .Trim();
    }

    /// <summary>
    /// Capitalizes the color string (e.g., "rojo fuego" -> "Rojo Fuego", "NEGRO" -> "Negro").
    /// </summary>
    private static string? FormatColor(string? color)
    {
        // Step 1: Guard clause for null, empty, or whitespace-only values
        if (string.IsNullOrWhiteSpace(color))
            return null;

        // Step 2: Trim whitespace and convert to lowercase as a clean baseline
        var clean = color.Trim().ToLowerInvariant();

        // Step 3: Apply TitleCase formatting to capitalize the first letter of each word
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(clean);
    }

    /// <summary>
    /// Converts text-based sizes to UPPERCASE (e.g., "xl" -> "XL", "m" -> "M").
    /// Leaves numeric sizes intact (e.g., "42", "10.5").
    /// </summary>
    private static string? FormatSize(string? size)
    {
        // Step 1: Guard clause for null, empty, or whitespace-only values
        if (string.IsNullOrWhiteSpace(size))
            return null;

        // Step 2: Remove leading and trailing whitespace
        var clean = size.Trim();

        // Step 3: Check if the size is numeric (e.g., "42", "38.5"); if so, preserve as-is
        if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
        {
            return clean;
        }

        // Step 4: Convert text-based size indicators (e.g., "xl", "s") to UPPERCASE
        return clean.ToUpperInvariant();
    }
}
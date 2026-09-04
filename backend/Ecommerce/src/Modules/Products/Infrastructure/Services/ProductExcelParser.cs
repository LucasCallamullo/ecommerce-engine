namespace Ecommerce.Products.Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.Interfaces;

/// <summary>
/// Infrastructure service responsible for parsing product data from Microsoft Excel (.xlsx) streams using MiniExcel.
/// Encapsulates technical file-reading dependencies away from the Application core using resilient type conversions.
/// </summary>
public class ProductExcelParser : IProductExcelParser
{
    /// <summary>
    /// Internal Data Transfer Object mapped directly to expected Excel column headers via MiniExcel attributes.
    /// Reads numerical fields as strings to prevent unhandled cast exceptions during deferred iteration.
    /// </summary>
    private class ProductExcelRow
    {
        [ExcelColumnName("name")]
        public string? Name { get; set; }

        [ExcelColumnName("price_ars")]
        public string? PriceArs { get; set; } // Read as string for safe parsing

        [ExcelColumnName("available")]
        public string? Available { get; set; } // Read as string to support "Si" / "No"

        [ExcelColumnName("stock")]
        public string? Stock { get; set; } // Read as string for safe parsing

        [ExcelColumnName("category")]
        public string? Category { get; set; }

        [ExcelColumnName("subcategory")]
        public string? Subcategory { get; set; }

        [ExcelColumnName("brand")]
        public string? Brand { get; set; }

        [ExcelColumnName("discount_ars")]
        public string? DiscountArs { get; set; } // Read as string for safe parsing

        [ExcelColumnName("description")]
        public string? Description { get; set; }

        [ExcelColumnName("image_url")]
        public string? ImageUrl { get; set; }

        [ExcelColumnName("image_url2")]
        public string? ImageUrl2 { get; set; }
    }

    /// <summary>
    /// Reads an incoming Excel file stream and projects its contents into domain-neutral application DTOs.
    /// </summary>
    /// <param name="fileStream">The memory stream containing the uploaded Excel spreadsheet binary data.</param>
    /// <returns>A collection of unvalidated <see cref="ProductImportDto"/> instances ready for business evaluation.</returns>
    public IEnumerable<ProductImportDto> ParseExcel(Stream fileStream)
    {
        var rows = fileStream.Query<ProductExcelRow>();

        return rows.Select(row => new ProductImportDto(
            Name: row.Name ?? string.Empty,
            PriceArs: ParseDecimal(row.PriceArs),
            Available: ParseBoolean(row.Available),
            Stock: ParseInt(row.Stock),
            Category: row.Category,
            Subcategory: row.Subcategory,
            Brand: row.Brand,
            DiscountArs: ParseInt(row.DiscountArs),
            Description: row.Description,
            ImageUrl: row.ImageUrl,
            ImageUrl2: row.ImageUrl2
        ));
    }

    /// <summary>
    /// Safely parses a string cell into a nullable decimal value supporting regional invariant formats.
    /// </summary>
    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Clean currency symbols or trailing spaces
        var cleanValue = value.Trim().Replace("$", string.Empty);

        if (decimal.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;

        // Fallback for comma decimal separators
        if (decimal.TryParse(cleanValue, NumberStyles.Any, new CultureInfo("es-AR"), out var arResult))
            return arResult;

        return null; // Triggers validation error in ProductImportService
    }

    /// <summary>
    /// Safely parses a string cell into a nullable integer value.
    /// </summary>
    private static int? ParseInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            return result;

        return null; // Triggers validation error in ProductImportService
    }

    /// <summary>
    /// Flexible boolean parser evaluating common Spanish and English affirmative cell values.
    /// </summary>
    private static bool ParseBoolean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true; // Default to available if cell is empty

        var clean = value.Trim().ToLowerInvariant();

        return clean is "si" or "sí" or "s" or "true" or "1" or "yes" or "y";
    }
}
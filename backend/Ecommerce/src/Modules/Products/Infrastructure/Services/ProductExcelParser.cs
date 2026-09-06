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
using Ecommerce.Shared.Common.Extensions;

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

        [ExcelColumnName("group_code")]
        public string? GroupProductVariant { get; set; }

        [ExcelColumnName("size")]
        public string? Size { get; set; }

        [ExcelColumnName("color")]
        public string? Color { get; set; }

        [ExcelColumnName("color_name")]
        public string? ColorName { get; set; }

        [ExcelColumnName("available")]
        public string? Available { get; set; } // Read as string to support "Si" / "No"

        [ExcelColumnName("stock")]
        public string? Stock { get; set; } // Read as string for safe parsing

        //! Multi-alias for Price: maps price_ars and price
        [ExcelColumnName("price_ars")]
        public string? PriceArsRaw { get; set; }    // Read as string for safe parsing

        [ExcelColumnName("price")]
        public string? PriceRaw { get; set; }    // Read as string for safe parsing

        // Propiedad calculada: intenta con "price_ars" y si es null/empty usa "price"
        public string? PriceArs => ResolveFirstNonEmpty(PriceArsRaw, PriceRaw);
    
        //! Multi-alias for Cost: maps cost_ars and cost
        [ExcelColumnName("cost_ars")]
        public string? UnitCostArsRaw { get; set; }    // Read as string for safe parsing

        [ExcelColumnName("cost")]
        public string? UnitCostRaw { get; set; }

        public string? UnitCostArs => ResolveFirstNonEmpty(UnitCostArsRaw, UnitCostRaw);
            
        // ! Name of Fk to related    
        [ExcelColumnName("category")]
        public string? Category { get; set; }

        [ExcelColumnName("subcategory")]
        public string? Subcategory { get; set; }

        [ExcelColumnName("brand")]
        public string? Brand { get; set; }

        //! Multi-alias for Discount: maps discount_ars, discount and descuento
        [ExcelColumnName("discount_ars")]
        public string? DiscountArsRaw { get; set; }    // Read as string for safe parsing

        [ExcelColumnName("discount")]
        public string? DiscountRaw { get; set; }    // Read as string for safe parsing

        [ExcelColumnName("descuento")]
        public string? DescuentoRaw { get; set; }    // Read as string for safe parsing

        // Propiedad expuesta a ParseExcel
        public string? DiscountPercentageArs => ResolveFirstNonEmpty(DiscountArsRaw, DiscountRaw, DescuentoRaw);

        // ! Other Values
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
            Name: row.Name.Sanitize() ?? string.Empty,
            GroupProduct: row.GroupProductVariant.Sanitize(),

            Size: row.Size.Sanitize(),
            Color: row.Color.Sanitize(),
            ColorName: row.ColorName.Sanitize(),
            IsActive: ParseBoolean(row.Available),
            Description: row.Description.Sanitize(),

            // Check Fields MUST HAS VALUE
            PriceArs: ParseDecimal(row.PriceArs),
            UnitCostArs: ParseDecimal(row.UnitCostArs),
            DiscountPercentageArs: ParseInt(row.DiscountPercentageArs),
            Stock: ParseInt(row.Stock),

            Category: row.Category.Sanitize(),
            Subcategory: row.Subcategory.Sanitize(),
            Brand: row.Brand.Sanitize(),
            ImageUrl: row.ImageUrl.Sanitize(),
            ImageUrl2: row.ImageUrl2.Sanitize()
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

    /// <summary>
    /// Evaluates a collection of candidate column string values in order and returns the first non-null, non-whitespace entry.
    /// Used to resolve column name aliases (e.g., "discount_ars", "discount", "descuento") into a single authoritative value.
    /// </summary>
    /// <param name="values">An array of candidate string values retrieved from various mapped Excel column aliases.</param>
    /// <returns>
    /// The first non-empty <see cref="string"/> value found among the candidates; 
    /// otherwise, <c>null</c> if all candidate values are null or contain only whitespace.
    /// </returns>
    private static string? ResolveFirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
}
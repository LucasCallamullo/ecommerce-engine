namespace Ecommerce.Products.Application.Interfaces;

using System.Collections.Generic;

using Ecommerce.Products.Application.DTOs.Request;

/// <summary>
/// Defines the contract for parsing raw Microsoft Excel (.xlsx) file streams into domain-neutral import DTOs.
/// Decouples spreadsheet parsing libraries from application business logic.
/// </summary>
public interface IProductExcelParser
{
    /// <summary>
    /// Reads and converts an Excel spreadsheet stream into a sequence of clean <see cref="ProductImportDto"/> records.
    /// </summary>
    /// <param name="fileStream">The readable binary stream containing the uploaded Excel file data.</param>
    /// <returns>An enumerable collection of unvalidated product import records extracted from the spreadsheet rows.</returns>
    IEnumerable<ProductImportDto> ParseExcel(Stream fileStream);
}
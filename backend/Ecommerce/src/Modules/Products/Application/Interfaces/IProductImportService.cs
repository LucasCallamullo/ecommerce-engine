namespace Ecommerce.Products.Application.Interfaces;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Products.Application.DTOs.Response;

/// <summary>
/// Defines business orchestration operations for bulk product imports.
/// </summary>
public interface IProductImportService
{
    /// <summary>
    /// Asynchronously processes a spreadsheet file stream, validating rows and persisting products into the catalog.
    /// </summary>
    /// <param name="fileStream">The incoming Excel file stream containing product rows.</param>
    /// <param name="cancellationToken">A token to observe while awaiting task completion.</param>
    /// <returns>A detailed execution report containing record counts and validation errors.</returns>
    Task<ProductImportResultDto> ImportFromExcelAsync(Stream fileStream, CancellationToken cancellationToken = default);
}
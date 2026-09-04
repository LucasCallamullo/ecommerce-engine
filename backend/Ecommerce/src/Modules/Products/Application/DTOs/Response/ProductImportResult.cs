namespace Ecommerce.Products.Application.DTOs.Response;

using System.Collections.Generic;

/// <summary>
/// Output payload detailing execution statistics and row-level error messages resulting from a bulk import operation.
/// </summary>
/// <param name="TotalRecords">Total number of data rows evaluated from the input file.</param>
/// <param name="SuccessfulCount">Number of products successfully mapped and persisted into the database.</param>
/// <param name="FailedCount">Number of rows that failed validation rules or lookups.</param>
/// <param name="Errors">Read-only list of formatted error details per row.</param>
public record ProductImportResultDto(
    int TotalRecords,
    int SuccessfulCount,
    int FailedCount,
    IReadOnlyCollection<string> Errors
);
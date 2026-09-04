namespace Ecommerce.Shared.Responses;

using System;
using System.Collections.Generic;

/// <summary>
/// Encapsulates a paginated result set containing data items alongside metadata for frontend navigation.
/// </summary>
/// <typeparam name="T">The type of items contained within the page result.</typeparam>
/// <param name="Items">The read-only collection of elements retrieved for the target page.</param>
/// <param name="TotalCount">The total number of matching elements across all pages in the data source.</param>
/// <param name="PageNumber">The current page index (1-based index).</param>
/// <param name="PageSize">The maximum number of items allocated per page.</param>
public record PagedResultDto<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
)
{
    /// <summary>
    /// Gets the total number of calculated pages based on total count and page size.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Gets a value indicating whether a subsequent page is available.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Gets a value indicating whether a preceding page exists.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
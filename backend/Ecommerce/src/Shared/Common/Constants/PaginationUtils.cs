namespace Ecommerce.Shared.Common.Constants;

/// <summary>
/// Provides utility methods and global defaults for normalizing pagination parameters across the application.
/// </summary>
public static class PaginationUtils
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 50; // Balanced for 2, 3, 4 column responsive UI grids
    public const int DefaultMaxPageSize = 100;

    /// <summary>
    /// Normalizes and clamps raw page number and page size values against configurable pagination limits.
    /// </summary>
    /// <param name="pageNumber">The raw page index requested by the client.</param>
    /// <param name="pageSize">The raw page size requested by the client.</param>
    /// <param name="defaultSize">Optional override for the default page size.</param>
    /// <param name="maxSize">Optional override for the maximum allowed page size limit.</param>
    /// <returns>A tuple containing the sanitized <c>PageNumber</c> and <c>PageSize</c>.</returns>
    public static (int PageNumber, int PageSize) Normalize(
        int pageNumber, 
        int pageSize, 
        int defaultSize = DefaultPageSize, 
        int maxSize = DefaultMaxPageSize)
    {
        var cleanPageNumber = pageNumber < 1 
            ? DefaultPageNumber 
            : pageNumber;

        var cleanPageSize = pageSize switch
        {
            <= 0 => defaultSize,
            _ when pageSize > maxSize => maxSize,
            _ => pageSize
        };

        return (cleanPageNumber, cleanPageSize);
    }
}
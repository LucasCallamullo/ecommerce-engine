namespace Ecommerce.Products.Application.DTOs.Request;

using Ecommerce.Shared.Common.Constants;
using Ecommerce.Shared.Common.Extensions; 

/// <summary>
/// Criteria parameters used to filter, sort, and paginate product catalog queries.
/// </summary>
public record ProductFilterQuery(
    int? CategoryId = null,
    int? SubcategoryId = null,
    int? BrandId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SearchTerm = null,
    string? SortBy = null,
    int PageNumber = PaginationUtils.DefaultPageNumber,
    int PageSize = PaginationUtils.DefaultPageSize
)
{
    // Cleans and normalizes the search term (e.g., "remera" -> "remera", " " -> null)
    public string? SearchTerm { get; init; } = SearchTerm.Sanitize();

    // Sanitizes and normalizes snake_case sort keys (e.g., "  PRICE_ASC " -> "price_asc")
    public string? SortBy { get; init; } = SortBy.Sanitize()?.ToLowerInvariant();
}
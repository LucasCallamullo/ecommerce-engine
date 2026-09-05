namespace Ecommerce.Products.Application.Interfaces;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Shared.Responses;

/// <summary>
/// Application service handling read-only queries, filtering, and pagination for the product catalog.
/// </summary>
public interface IProductQueryService
{
    /// <summary>
    /// Asynchronously retrieves a paginated set of active products matching the provided filter criteria.
    /// </summary>
    /// <param name="filter">The query parameters containing filter bounds, sorting, and pagination options.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task containing the <see cref="PagedResultDto{T}"/> with <see cref="ProductResponse"/> items.</returns>
    Task<PagedResultDto<ProductResponse>> GetPagedProductsAsync(
        ProductFilterQuery filter, 
        CancellationToken ct = default);
}
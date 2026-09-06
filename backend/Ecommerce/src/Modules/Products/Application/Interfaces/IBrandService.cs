namespace Ecommerce.Products.Application.Interfaces;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Domain.Entities;

using Ecommerce.Shared.Responses;

/// <summary>
/// Service contract managing product brand business operations and data persistence.
/// </summary>
public interface IBrandService
{
    /// <summary>
    /// Retrieves a single brand entity by its URL-friendly slug.
    /// Throws an <see cref="AppException"/> if the entity is not found or inactive.
    /// </summary>
    /// <param name="slug">The URL-friendly slug of the target brand.</param>
    Task<Brand> GetEntityBySlugAsync(string slug, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously retrieves a tracked or non-tracked <see cref="Brand"/> entity by its unique identifier.
    /// Throws an <see cref="AppException"/> if the entity is not found or inactive.
    /// </summary>
    /// <param name="id">The unique primary key identifier of the brand.</param>
    /// <returns>A task representing the asynchronous operation, containing the resolved <see cref="Brand"/> entity.</returns>
    Task<Brand> GetEntityByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Asynchronously retrieves a paginated list of active products associated with a specific brand slug.
    /// </summary>
    /// <param name="brandSlug">The URL-friendly slug of the target brand.</param>
    /// <param name="filter">The criteria parameters containing filter bounds, sorting, and pagination options.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a <see cref="PagedResultDto{T}"/> 
    /// filled with <see cref="ProductResponse"/> payloads.
    /// </returns>
    Task<PagedResultDto<ProductResponse>> GetProductsByBrandSlugAsync(
        string brandSlug,
        ProductFilterQuery filter,
        CancellationToken ct = default);

    // * ===============================================
    // *         METHODS --> GET
    // * ===============================================

    /// <summary>
    /// Retrieves a basic brand representation by its unique identifier.
    /// </summary>
    /// <param name="id">The unique primary key of the brand entity.</param>
    /// <returns>A <see cref="BrandResponse"/> payload containing essential brand details.</returns>
    Task<BrandResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves comprehensive brand metadata by its unique identifier for detailed views.
    /// </summary>
    /// <param name="id">The unique primary key of the brand entity.</param>
    /// <returns>A <see cref="BrandDetailResponse"/> payload containing complete brand properties.</returns>
    Task<BrandDetailResponse> GetByIdDetailAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active and non-deleted brands available in the product catalog.
    /// </summary>
    /// <returns>A collection of <see cref="BrandResponse"/> items.</returns>
    Task<IEnumerable<BrandResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    // * ===============================================
    // *         METHODS --> POST | UPDATE | DELETE
    // * ===============================================

    /// <summary>
    /// Creates a new brand in the catalog, automatically generating its URL-friendly slug.
    /// </summary>
    /// <param name="request">The data payload containing brand creation details.</param>
    /// <returns>The newly created <see cref="BrandResponse"/> record.</returns>
    Task<BrandDetailResponse> CreateAsync(BrandCreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing brand's properties and synchronizes its URL-friendly slug.
    /// </summary>
    /// <param name="id">The unique primary key of the brand to update.</param>
    /// <param name="request">The data payload containing updated brand values.</param>
    /// <returns>The updated <see cref="BrandResponse"/> record.</returns>
    Task<BrandDetailResponse> UpdateAsync(int id, BrandUpdateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a logical soft deletion on a brand entity by setting its deleted flag.
    /// </summary>
    /// <param name="id">The unique primary key of the brand to logically remove.</param>
    /// <returns><c>true</c> if the soft deletion was successful.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
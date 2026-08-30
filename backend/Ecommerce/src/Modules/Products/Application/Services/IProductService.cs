using System.Net;
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Exceptions;

namespace Ecommerce.Products.Application.Services;

/// <summary>
/// Defines business logic contracts for managing products and their lifecycle.
/// </summary>
/// 
/// <param name="cancellationToken">Cancellation token to abort the operation if requested.</param>
public interface IProductService
{
    /// <summary>
    /// Retrieves the domain <see cref="Product"/> entity by its unique identifier for internal service processing.
    /// </summary>
    /// <param name="id">The unique identifier of the product entity.</param>
    /// <returns>The active <see cref="Product"/> domain entity.</returns>
    /// <exception cref="AppException">
    /// Thrown with status code <see cref="HttpStatusCode.NotFound"/> when the product does not exist or is marked as deleted.
    /// </exception>
    Task<Product> GetEntityByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether an active product exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product entity.</param>
    /// <param name="cancellationToken">Cancellation token to abort the operation if requested.</param>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures a product exists and is active; otherwise throws an <see cref="AppException"/> (404 Not Found).
    /// </summary>
    Task EnsureExistsAsync(int id, CancellationToken cancellationToken = default);

    // ? CRUD METHOS --> 

    /// <summary>
    /// Retrieves detailed product data by ID (including Category, Brand, and active Variants).
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The <see cref="ProductDetailResponse"/> representing the requested product.</returns>
    /// <exception cref="AppException">
    /// Thrown with status code <see cref="HttpStatusCode.NotFound"/> when the product does not exist or is marked as deleted.
    /// </exception>
    Task<ProductDetailResponse> GetByIdDetailAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all active (non-deleted) products.
    /// </summary>
    /// <returns>A collection of <see cref="ProductResponse"/> representing active products.</returns>
    Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves basic product data by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The <see cref="ProductResponse"/> representing the requested product.</returns>
    /// <exception cref="AppException">
    /// Thrown with status code <see cref="HttpStatusCode.NotFound"/> when the product does not exist or is marked as deleted.
    /// </exception>
    Task<ProductResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Maps, generates slug, and persists a new product with its variants.
    /// </summary>
    /// <param name="request">Data transfer object containing creation details for the product.</param>
    /// <returns>The created <see cref="ProductResponse"/>.</returns>
    /// <exception cref="AppException">
    /// Thrown with status code <see cref="HttpStatusCode.BadRequest"/> if validation fails or a duplicate slug/SKU is detected.
    /// </exception>
    Task<ProductResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies soft delete (<c>IsDeleted = true</c>) to a product and its associated variants.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <returns><c>true</c> if the soft deletion completed successfully.</returns>
    /// <exception cref="AppException">
    /// Thrown with status code <see cref="HttpStatusCode.NotFound"/> when the product does not exist or is already deleted.
    /// </exception>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
namespace Ecommerce.Products.Application.Interfaces;

using System.Linq.Expressions;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Domain.Entities;

using Ecommerce.Shared.Exceptions;

/// <summary>
/// Business logic contract for product catalog operations and lifecycle management.
/// </summary>
public interface IProductService
{
    //? =====================================================================
    //?         METHODS --> Entity
    //? =====================================================================

    /// <summary>
    /// Asynchronously retrieves a tracked full <see cref="Product"/> aggregate root entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique primary key identifier of the product.</param>
    /// <returns>A task containing the fully loaded <see cref="Product"/> entity.</returns>
    /// <exception cref="AppException">Thrown with HTTP 404 status when no matching product is found.</exception>
    Task<Product> GetEntityByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously retrieves a custom projection or partial <see cref="Product"/> entity without tracking, 
    /// executing a lightweight SQL query containing only the specified scalar columns.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the projected result (e.g., a tuple, a DTO, or a partial <see cref="Product"/> instance).
    /// </typeparam>
    /// <param name="id">The unique primary key identifier of the product.</param>
    /// <param name="selector">An expression defining the properties to project from the database entity.</param>
    /// <returns>A task containing the projected result payload.</returns>
    /// <exception cref="AppException">Thrown with HTTP 404 status when no matching product is found.</exception>
    Task<T> GetEntityByIdAsync<T>(
        int id, 
        Expression<Func<Product, T>> selector, 
        CancellationToken ct = default);

    /// <summary>
    /// Asynchronously determines whether an active, non-deleted product exists with the specified primary key identifier.
    /// </summary>
    /// <param name="id">The unique primary key identifier of the product.</param>
    /// <returns><c>true</c> if the product exists and is not deleted; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously determines whether any active, non-deleted product satisfies the specified predicate condition.
    /// </summary>
    /// <param name="predicate">A expression to test each product element against a condition.</param>
    /// <returns><c>true</c> if any product satisfies the condition; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsAsync(Expression<Func<Product, bool>> predicate, CancellationToken ct = default);

    //? =====================================================================
    //?         METHODS --> GET
    //? =====================================================================

    /// <summary>
    /// Retrieves a detailed product payload by its unique URL-friendly slug.
    /// </summary>
    /// <param name="slug">The URL-friendly slug identifier of the product.</param>
    /// <returns>A task containing the <see cref="ProductDetailResponse"/> details.</returns>
    Task<ProductDetailResponse> GetBySlugAsync(string slug, CancellationToken ct = default);

    /// <summary>Retrieves basic product data by ID.</summary>
    /// <exception cref="AppException">404 Not Found if the product does not exist or is marked as deleted.</exception>
    Task<ProductResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves full product tree (Category, Brand, Variants, Images) by ID.</summary>
    /// <exception cref="AppException">404 Not Found if the product does not exist or is marked as deleted.</exception>
    Task<ProductDetailResponse> GetByIdDetailAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all active non-deleted products.</summary>
    Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    //? =====================================================================
    //?         METHODS --> CREATE | DELETE | UPDATE
    //? =====================================================================

    /// <summary>Persists a new product alongside its initial variants.</summary>
    /// <exception cref="AppException">400 Bad Request if validation fails or duplicate slug/SKU is detected.</exception>
    Task<ProductDetailResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>Update a product alongside its initial variants and images.</summary>
    /// <exception cref="AppException">
    /// 400 Bad Request if validation fails or duplicate slug/SKU is detected.
    /// 404 Not Found if product does not exist.
    /// </exception>
    Task<ProductResponse> UpdateAsync(int id, ProductUpdateRequest request, CancellationToken cancellationToken = default);

    /// <summary>Performs a logical soft delete (IsDeleted = true) on a product and its variants.</summary>
    /// <exception cref="AppException">404 Not Found if product does not exist | 400 Bad Request if already deleted.</exception>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
using System.Net;
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Exceptions;

namespace Ecommerce.Products.Application.Interfaces;

/// <summary>Business logic contract for product catalog operations and lifecycle management.</summary>
public interface IProductService
{
    /// <summary>Retrieves the domain Product entity by ID for internal module processing.</summary>
    /// <exception cref="AppException">404 Not Found if the product does not exist or is marked as deleted.</exception>
    Task<Product> GetEntityByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Checks whether an active product exists by ID.</summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Ensures a product exists and is active; otherwise throws 404 Not Found.</summary>
    /// <exception cref="AppException">404 Not Found if the product does not exist or is marked as deleted.</exception>
    Task EnsureExistsAsync(int id, CancellationToken cancellationToken = default);

    //? =====================================================================
    //?         GET METHODS
    //? =====================================================================

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
    Task<ProductResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default);

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
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;

namespace Ecommerce.Products.Application.Services;

public interface IProductService
{
    // Retrieves detailed product data by ID (including Category, Brand, and Variants).
    // Throws AppException (404) if the product does not exist or is deleted.
    Task<ProductDetailResponse> GetByIdDetailAsync(int id, CancellationToken cancellationToken = default);

    // Retrieves all active (non-deleted) products.
    Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    // Retrieves basic product data by ID.
    // Throws AppException (404) if the product does not exist or is deleted.
    Task<ProductResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Maps, generates slug, and persists a new product with its variants.
    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);

    // Applies soft delete (IsDeleted = true) to a product.
    // Throws AppException (404) if the product does not exist or is already deleted.
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
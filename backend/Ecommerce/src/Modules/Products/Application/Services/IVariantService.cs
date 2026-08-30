// Application/Services/IVariantService.cs
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Exceptions;

namespace Ecommerce.Products.Application.Services;

/// <summary>
/// Defines business logic contracts for managing product variants.
/// Handles retrieval, persistence, mapping, and soft-deletion of product variants.
/// </summary>
/// <param name="cancellationToken">Cancellation token to abort the operation if requested.</param>
public interface IVariantService
{
    /// <summary>
    /// Retrieves all active (non-deleted) product variants.
    /// </summary>
    /// <returns>A collection of <see cref="ProductVariantResponse"/> representing active variants.</returns>
    Task<IEnumerable<ProductVariantResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed product variant data by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product variant.</param>
    /// <returns>The <see cref="ProductVariantResponse"/> representing the requested variant.</returns>
    /// <exception cref="AppException">
    /// Thrown with status code <see cref="HttpStatusCode.NotFound"/> when the variant with the specified ID does not exist or is marked as deleted.
    /// Example: <c>throw new AppException($"Product variant with ID {id} was not found.", HttpStatusCode.NotFound);</c>
    /// </exception>
    Task<ProductVariantResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies soft deletion to a product variant by setting <c>IsDeleted = true</c>.
    /// </summary>
    /// <param name="id">The unique identifier of the product variant to delete.</param>
    /// <returns><c>true</c> if the soft deletion completed successfully.</returns>
    /// <exception cref="AppException">
    /// Thrown with status code <see cref="HttpStatusCode.NotFound"/> 
    /// when the variant with the specified ID does not exist or is already deleted.
    /// </exception>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and persists a new product variant.
    /// </summary>
    /// <param name="request">Data transfer object containing creation details for the variant.</param>
    /// <returns>The created <see cref="ProductVariantResponse"/>.</returns>
    /// <exception cref="AppException">
    /// Thrown with status code <see cref="HttpStatusCode.BadRequest"/> if validation fails or a duplicate SKU/attributes are detected.
    /// </exception>
    Task<ProductVariantResponse> CreateAsync(
        int productId,
        ProductCreateVariantRequest request, 
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates and persists a new product variant.
    /// </summary>
    /// <param name="request">Data transfer object containing creation details for the variant.</param>
    /// <returns>The created <see cref="ProductVariantResponse"/>.</returns>
    /// <exception cref="AppException">
    /// Thrown with status code <see cref="HttpStatusCode.BadRequest"/> if validation fails or a duplicate SKU/attributes are detected.
    /// </exception>
    Task<ProductVariantResponse> UpdateAsync(
        int productId,
        int id,
        ProductVariantUpdateRequest request, 
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates ProductVariant entities from request DTOs and associates them with a product.
    /// </summary>
    /// <param name="variantRequests">List of variant request DTOs</param>
    /// <param name="product">The parent product entity</param>
    /// <returns>List of created ProductVariant entities</returns>
    List<ProductVariant> CreateVariantsFromRequests(
        List<ProductCreateVariantRequest> variantRequests, 
        Product product);

    /// <summary> Generates a unique SKU for a variant. </summary>
    string GenerateSku();
}
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Exceptions;

namespace Ecommerce.Products.Application.Services;

/// <summary>Defines business logic contracts for managing product variants.</summary>
public interface IVariantService
{
    //? =====================================================================
    //?         INTERNAL HELPER METHODS
    //? =====================================================================

    /// <summary>Maps variant request DTOs into domain entity instances associated with a parent product.</summary>
    /// <returns>A list of instantiated <see cref="ProductVariant"/> entities.</returns>
    List<ProductVariant> CreateVariantsFromRequests(
        List<ProductCreateVariantRequest> variantRequests, 
        Product product
    );

    /// <summary>Generates a unique catalog SKU string for a variant.</summary>
    /// <returns>A unique SKU string representation.</returns>
    string GenerateSku();

    //? =====================================================================
    //?         GET METHODS
    //? =====================================================================

    /// <summary>Retrieves all active variants associated with a specific product.</summary>
    /// <returns>A collection of <see cref="ProductVariantResponse"/> belonging to the specified product.</returns>
    /// <exception cref="AppException">404 Not Found if the parent product does not exist or is deleted.</exception>
    Task<IEnumerable<ProductVariantResponse>> GetVariantsByProductId(
        int productId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Retrieves all active product variants across the catalog.</summary>
    /// <returns>A collection of all active <see cref="ProductVariantResponse"/>.</returns>
    Task<IEnumerable<ProductVariantResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves detailed product variant data by its unique identifier.</summary>
    /// <returns>The <see cref="ProductVariantResponse"/> representing the requested variant.</returns>
    /// <exception cref="AppException">404 Not Found if the variant does not exist or is marked as deleted.</exception>
    Task<ProductVariantResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    //? =====================================================================
    //?         METHODS --> POST / UPDATE / DELETE 
    //? =====================================================================

    /// <summary>Creates and persists a new product variant under a parent product.</summary>
    /// <returns>The newly created <see cref="ProductVariantResponse"/>.</returns>
    /// <exception cref="AppException">404 Not Found if parent product is missing | 400 Bad Request if validation or SKU fails.</exception>
    Task<ProductVariantResponse> CreateAsync(
        int productId,
        ProductCreateVariantRequest request, 
        CancellationToken cancellationToken = default
    );

    /// <summary>Updates an existing product variant's data.</summary>
    /// <returns>The updated <see cref="ProductVariantResponse"/>.</returns>
    /// <exception cref="AppException">404 Not Found if variant/product is missing | 400 Bad Request if validation fails.</exception>
    Task<ProductVariantResponse> UpdateAsync(
        int productId,
        int id,
        ProductVariantUpdateRequest request, 
        CancellationToken cancellationToken = default
    );

    /// <summary>Applies logical soft deletion to a product variant by setting IsDeleted to true.</summary>
    /// <returns><c>true</c> if the soft deletion was successful.</returns>
    /// <exception cref="AppException">404 Not Found if variant does not exist | 400 Bad Request if already deleted.</exception>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
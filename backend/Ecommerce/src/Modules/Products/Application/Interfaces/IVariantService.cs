using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Exceptions;

namespace Ecommerce.Products.Application.Interfaces;

/// <summary>Defines business logic contracts for managing product variants.</summary>
public interface IVariantService
{
    //? =====================================================================
    //?         INTERNAL HELPER METHODS
    //? =====================================================================

    /// <summary>Maps variant request DTOs into domain entity instances associated with a parent product.</summary>
    /// <returns>A list of instantiated <see cref="VariantResponse"/> entities.</returns>
    List<ProductVariant> CreateVariantsFromRequests(
        List<ProductCreateVariantRequest> variantRequests, 
        Product product
    );

    //? =====================================================================
    //?         GET METHODS
    //? =====================================================================

    /// <summary>Retrieves all active variants associated with a specific product.</summary>
    /// <returns>A collection of <see cref="VariantResponse"/> belonging to the specified product.</returns>
    /// <exception cref="AppException">404 Not Found if the parent product does not exist or is deleted.</exception>
    Task<IEnumerable<VariantResponse>> GetVariantsByProductId(int productId, CancellationToken ct = default);

    /// <summary>Retrieves all active product variants across the catalog.</summary>
    /// <returns>A collection of all active <see cref="VariantResponse"/>.</returns>
    Task<IEnumerable<VariantResponse>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Retrieves detailed product variant data by its unique identifier.</summary>
    /// <returns>The <see cref="VariantResponse"/> representing the requested variant.</returns>
    /// <exception cref="AppException">404 Not Found if the variant does not exist or is marked as deleted.</exception>
    Task<VariantResponse> GetByIdAsync(int id, CancellationToken ct = default);

    //? =====================================================================
    //?         METHODS --> POST / UPDATE / DELETE 
    //? =====================================================================

    /// <summary>Creates and persists a new product variant under a parent product.</summary>
    /// <returns>The newly created <see cref="VariantDetailResponse"/>.</returns>
    /// <exception cref="AppException">
    /// 404 Not Found if parent product is missing | 400 Bad Request if validation or SKU fails.
    /// </exception>
    Task<VariantDetailResponse> CreateAsync(
        int productId,
        ProductCreateVariantRequest request, 
        CancellationToken ct = default
    );

    /// <summary>Updates an existing product variant's data.</summary>
    /// <returns>The updated <see cref="VariantDetailResponse"/>.</returns>
    /// <exception cref="AppException">404 Not Found if variant/product is missing | 400 Bad Request if validation fails.</exception>
    Task<VariantDetailResponse> UpdateAsync(
        int productId,
        int id,
        ProductVariantUpdateRequest request, 
        CancellationToken ct = default
    );

    /// <summary>Applies logical soft deletion to a product variant by setting IsDeleted to true.</summary>
    /// <returns><c>true</c> if the soft deletion was successful.</returns>
    /// <exception cref="AppException">404 Not Found if variant does not exist | 400 Bad Request if already deleted.</exception>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
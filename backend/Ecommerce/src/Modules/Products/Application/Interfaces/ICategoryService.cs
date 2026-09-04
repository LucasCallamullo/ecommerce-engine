namespace Ecommerce.Products.Application.Interfaces;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;

/// <summary>
/// Application service contract defining business operations for catalog categories and subcategories.
/// </summary>
public interface ICategoryService
{
    //* =====================================================================
    //*         METHODS --> GET
    //* =====================================================================

    /// <summary>Retrieves all active root categories (where ParentCategoryId is null).</summary>
    Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves lightweight category summary details by its unique identifier.</summary>
    Task<CategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves comprehensive category metadata including parent context by its unique identifier.</summary>
    Task<CategoryDetailResponse> GetByIdDetailAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all active subcategories linked to a specific parent category ID.</summary>
    Task<IEnumerable<CategoryResponse>> GetSubcategoriesByParentIdAsync(int parentId, CancellationToken cancellationToken = default);

    //* =====================================================================
    //*         METHODS --> POST / PUT / DELETE 
    //* =====================================================================

    /// <summary>Creates a new category or subcategory entry.</summary>
    Task<CategoryResponse> CreateAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing category or subcategory entry.</summary>
    Task<CategoryResponse> UpdateAsync(int id, CategoryUpdateRequest request, CancellationToken cancellationToken = default);

    /// <summary>Performs a logical soft deletion on a category or subcategory.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
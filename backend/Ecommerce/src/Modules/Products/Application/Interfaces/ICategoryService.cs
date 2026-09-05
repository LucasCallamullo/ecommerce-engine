namespace Ecommerce.Products.Application.Interfaces;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;

using Ecommerce.Shared.Responses;
using Ecommerce.Shared.Common.Constants;
using Ecommerce.Products.Domain.Entities;

/// <summary>
/// Application service contract defining business operations for catalog categories and subcategories.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Asynchronously retrieves a paginated list of active products filtered by category and optional subcategory slugs.
    /// </summary>
    /// <param name="categorySlug">The URL-friendly slug of the primary root category.</param>
    /// <param name="subcategorySlug">The optional URL-friendly slug of the nested subcategory.</param>
    /// <param name="ProductFilterQuery">BasedQueryParams.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a <see cref="PagedResultDto{T}"/> 
    /// filled with <see cref="ProductResponse"/> payloads.
    /// </returns>
    Task<PagedResultDto<ProductResponse>> GetProductsByCategorySlugAsync(
        string categorySlug,
        string? subcategorySlug,
        ProductFilterQuery filter,
        CancellationToken ct = default);

    /// <summary>
    /// Asynchronously retrieves all active root categories alongside their associated nested subcategories.
    /// Optimized for dynamic navigation UI components.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of lightweight root categories 
    /// with their subcategories.
    /// </returns>
    Task<IEnumerable<CategoryWithSubcategories>> GetCategoriesWithSubcategoriesAsync(CancellationToken ct = default);

    /// <summary>Retrieves entity category by its unique identifier.</summary>
    Task<Category> GetEntityBySlugAsync(string slug, int? parentId, CancellationToken ct = default);

    //* =====================================================================
    //*         METHODS --> GET
    //* =====================================================================

    /// <summary>Retrieves all active root categories (where ParentCategoryId is null).</summary>
    Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Retrieves lightweight category summary details by its unique identifier.</summary>
    Task<CategoryResponse> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Retrieves comprehensive category metadata including parent context by its unique identifier.</summary>
    Task<CategoryDetailResponse> GetByIdDetailAsync(int id, CancellationToken ct = default);

    /// <summary>Retrieves all active subcategories linked to a specific parent category ID.</summary>
    Task<IEnumerable<CategoryResponse>> GetSubcategoriesByParentIdAsync(int parentId, CancellationToken ct = default);

    //* =====================================================================
    //*         METHODS --> POST / PUT / DELETE 
    //* =====================================================================

    /// <summary>Creates a new category or subcategory entry.</summary>
    Task<CategoryDetailResponse> CreateAsync(CategoryCreateRequest request, CancellationToken ct = default);

    /// <summary>Updates an existing category or subcategory entry.</summary>
    Task<CategoryDetailResponse> UpdateAsync(int id, CategoryUpdateRequest request, CancellationToken ct = default);

    /// <summary>Performs a logical soft deletion on a category or subcategory.</summary>
    Task DeleteAsync(int id, CancellationToken ct = default);
}
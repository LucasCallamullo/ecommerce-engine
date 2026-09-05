namespace Ecommerce.Products.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;

using Ecommerce.Shared.API;
using Ecommerce.Shared.Responses;
using Ecommerce.Shared.Common.Constants;

/// <summary>REST API Controller managing catalog category and subcategory operations.</summary>
[Route("api/v1/categories")]
public class CategoriesController(ICategoryService categoryService) : ApiControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    /// <summary>Retrieves paginated products filtered by root category slug.</summary>
    [HttpGet("{categorySlug}")]
    [ProducesResponseType(typeof(PagedResultDto<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsByCategorySlug(
        string categorySlug,
        [FromQuery] ProductFilterQuery filter,
        CancellationToken ct = default)
    {
        var result = await _categoryService.GetProductsByCategorySlugAsync(
            categorySlug, subcategorySlug: null, filter, ct);

        return Ok(result);
    }

    /// <summary>Retrieves paginated products filtered by root category and nested subcategory slugs.</summary>
    [HttpGet("{categorySlug}/{subcategorySlug}")]
    [ProducesResponseType(typeof(PagedResultDto<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsBySubcategorySlug(
        string categorySlug,
        string subcategorySlug,
        [FromQuery] ProductFilterQuery filter,
        CancellationToken ct = default)
    {
        var result = await _categoryService.GetProductsByCategorySlugAsync(
            categorySlug, subcategorySlug, filter, ct);

        return Ok(result);
    }

    /// <summary>Retrieves all active categories with subcategories childs.</summary>
    [HttpGet("subcategories")]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoriesWithChilds(CancellationToken ct)
    {
        var subcategories = await _categoryService.GetCategoriesWithSubcategoriesAsync(ct);
        return Ok(subcategories);
    }

    //* =====================================================================
    //*         METHODS --> GET BASIC
    //* =====================================================================

    /// <summary>Retrieves all active top-level (root) categories.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var categories = await _categoryService.GetAllAsync(ct);
        return Ok(categories);
    }

    /// <summary>Retrieves category details by its unique ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var category = await _categoryService.GetByIdAsync(id, ct);
        return Ok(category);
    }

    /// <summary>Retrieves category details by its unique ID.</summary>
    [HttpGet("{id:int}/detail")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdDetail(int id, CancellationToken ct)
    {
        var category = await _categoryService.GetByIdDetailAsync(id, ct);
        return Ok(category);
    }

    /// <summary>Retrieves all active subcategories for a specific parent category ID.</summary>
    [HttpGet("{parentId:int}/subcategories")]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubcategoriesByParentId(int parentId, CancellationToken ct)
    {
        var subcategories = await _categoryService.GetSubcategoriesByParentIdAsync(parentId, ct);
        return Ok(subcategories);
    }

    //* =====================================================================
    //*         METHODS --> POST / PUT / DELETE 
    //* =====================================================================

    /// <summary>Creates a new root category (or subcategory if ParentCategoryId is provided in payload).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CategoryCreateRequest request, 
        CancellationToken ct)
    {
        var response = await _categoryService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>Updates an existing category or subcategory by its ID.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, 
        [FromBody] CategoryUpdateRequest request, 
        CancellationToken ct)
    {
        var response = await _categoryService.UpdateAsync(id, request, ct);
        return Ok(response);
    }

    /// <summary>Performs a logical soft deletion on a category or subcategory by its ID.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _categoryService.DeleteAsync(id, ct);
        return NoContent();
    }
}
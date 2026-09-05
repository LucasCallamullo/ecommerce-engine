namespace Ecommerce.Products.Application.Services.Internals;

using System.Net;
using Mapster;
using Microsoft.EntityFrameworkCore;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Products.Domain.Entities;

using Ecommerce.Shared.Database;
using Ecommerce.Shared.Exceptions;
using Ecommerce.Shared.Responses;
using Ecommerce.Shared.Common.Extensions;

/// <summary>
/// Implements domain logic and data persistence operations for catalog categories and subcategories using Mapster.
/// </summary>
public class CategoryService(
    AppDbContext context,
    IProductQueryService productQueryService) : ICategoryService
{
    private readonly AppDbContext _context = context;
    private readonly IProductQueryService _productQueryService = productQueryService;

    //* =====================================================================
    //*         METHODS --> GET
    //* =====================================================================

    public async Task<IEnumerable<CategoryWithSubcategories>> GetCategoriesWithSubcategoriesAsync(
        CancellationToken ct = default)
    {
        return await _context.Set<Category>()
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == null && c.IsActive && !c.IsDeleted)
            .ProjectToType<CategoryWithSubcategories>()
            .ToListAsync(ct);
    }

    public async Task<PagedResultDto<ProductResponse>> GetProductsByCategorySlugAsync(
        string categorySlug,
        string? subcategorySlug,
        ProductFilterQuery filter,
        CancellationToken ct = default)
    {
        // Step 1: Resolve Category Entities (Ensures 404 if slug doesn't exist)
        var category = await GetEntityBySlugAsync(categorySlug, parentId: null, ct);

        Category? subcategory = null;
        if (!string.IsNullOrWhiteSpace(subcategorySlug))
            subcategory = await GetEntityBySlugAsync(subcategorySlug, parentId: category.Id, ct);

        // Step 2: Override the query filter with resolved Foreign Key IDs
        var categoryFilter = filter with
        {
            CategoryId = category.Id,
            SubcategoryId = subcategory?.Id
        };

        // Step 3: Delegate execution to ProductQueryService
        var pagedProducts = await _productQueryService.GetPagedProductsAsync(categoryFilter, ct);

        // Step 3: Attach Category metadata in ExtraData for UI Header
        var extraData = new Dictionary<string, object>
        {
            ["category"] = category.Adapt<CategoryResponse>()
        };

        if (subcategory != null)
        {
            extraData["subcategory"] = subcategory.Adapt<CategoryResponse>();
        }

        // Return new PagedResult containing the ExtraData context
        return pagedProducts with { ExtraData = extraData };
    }

    //* =====================================================================
    //*         METHODS --> GET Basics
    //* =====================================================================

    public async Task<Category> GetEntityBySlugAsync(string slug, int? parentId, CancellationToken ct = default)
    {
        return await _context.Set<Category>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Slug == slug && c.ParentCategoryId == parentId && c.IsActive, ct)
            ?? throw new AppException(
                $"Category with slug '{slug}' and ParentId '{parentId}' was not found.", HttpStatusCode.NotFound);
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken ct = default)
    {
        // Only recover Categories Parentss
        return await _context.Set<Category>()
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == null)
            .ProjectToType<CategoryResponse>()
            .ToListAsync(ct);
    }

    public async Task<CategoryResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Set<Category>()
            .AsNoTracking()
            .Where(c => c.Id == id)
            .ProjectToType<CategoryResponse>()
            .FirstOrDefaultAsync(ct)
            ?? throw new AppException($"Category with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<CategoryDetailResponse> GetByIdDetailAsync(int id, CancellationToken ct = default)
    {
        return await _context.Set<Category>()
            .AsNoTracking()
            .Include(c => c.ParentCategory)
            .Where(c => c.Id == id)
            .ProjectToType<CategoryDetailResponse>()
            .FirstOrDefaultAsync(ct)
            ?? throw new AppException($"Category with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<IEnumerable<CategoryResponse>> GetSubcategoriesByParentIdAsync(
        int parentId, CancellationToken ct = default)
    {
        var parentExists = await _context.Set<Category>()
            .AnyAsync(c => c.Id == parentId, ct);

        if (!parentExists)
            throw new AppException($"Parent category with ID {parentId} was not found.", HttpStatusCode.NotFound);

        return await _context.Set<Category>()
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == parentId)
            .ProjectToType<CategoryResponse>()
            .ToListAsync(ct);
    }

    //* =====================================================================
    //*         METHODS --> POST / PUT / DELETE 
    //* =====================================================================

    public async Task<CategoryDetailResponse> CreateAsync(
        CategoryCreateRequest request, CancellationToken ct = default)
    {
        // Step 1: Validate Parent Category Hierarchy (Enforces Max 2-Level Depth)
        if (request.ParentCategoryId.HasValue)
        {
            var parentId = request.ParentCategoryId.Value;

            // Ensure parent exists AND is a root category (cannot link a subcategory to another subcategory)
            var isRootParent = await _context.Set<Category>()
                .AnyAsync(c => c.Id == parentId && c.ParentCategoryId == null, ct);

            if (!isRootParent)
                throw new AppException(
                    $"Parent category with ID {parentId} was not found or is already a subcategory.", 
                    HttpStatusCode.BadRequest);
        }

        // Step 2: Generate and Validate Unique Slug
        // request.Name;    // is sanitized from dtos
        var slug = request.Name.ToSlug();

        var slugExists = await _context.Set<Category>()
            .AnyAsync(c => c.Slug == slug && c.ParentCategoryId == request.ParentCategoryId, ct);

        if (slugExists)
            throw new AppException(
                $"A category with the name '{request.Name}' already exists.", 
                HttpStatusCode.BadRequest);

        // Step 3: Map Entity with Sanitized Attributes
        // Step 1: Map clean payload directly to domain entity
        var category = request.Adapt<Category>();
        category.Slug = request.Name.ToSlug();

        // Step 5: Persistence & Response Mapping
        _context.Set<Category>().Add(category);
        await _context.SaveChangesAsync(ct);

        return category.Adapt<CategoryDetailResponse>();
    }

    public async Task<CategoryDetailResponse> UpdateAsync(int id, CategoryUpdateRequest request, CancellationToken ct = default)
    {
        // Step 1: Sanitize Primary Input
        // request.Name --> Sanitized from dto's
        var category = await _context.Set<Category>()
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new AppException($"Category with ID {id} was not found.", HttpStatusCode.NotFound);

        var newParentId = request.ParentCategoryId;

        // Step 2: Validate Parent Category Hierarchy Changes
        if (newParentId.HasValue)
        {
            // Rule 1: Self-referential Guard: A category cannot be its own parent
            if (newParentId.Value == id)
                throw new AppException("A category cannot be set as its own parent.", HttpStatusCode.BadRequest);

            // Rule 2: A Category that was created as a Root Category CANNOT become a Subcategory
            if (category.ParentCategoryId == null)
                throw new AppException(
                    "A root category cannot be converted into a subcategory.", HttpStatusCode.BadRequest);

            // Rule 3: If moving a Subcategory to a DIFFERENT parent, validate the new target parent
            if (newParentId != category.ParentCategoryId)
            {
                // Target parent must exist AND must be a Root Category (ParentCategoryId == null)
                var isRootParent = await _context.Set<Category>()
                    .AnyAsync(c => c.Id == newParentId.Value && c.ParentCategoryId == null, ct);

                if (!isRootParent)
                    throw new AppException(
                        $"Target parent category with ID {newParentId.Value} was not found or is not a root category.", 
                        HttpStatusCode.BadRequest);
            }
        }

        // Step 3: Validate Unique Slug (if Name OR Parent Category changed)
        var cleanName = request.Name ?? category.Name;
        var hasNameChanged = !string.Equals(category.Name, cleanName, StringComparison.OrdinalIgnoreCase);
        var hasParentChanged = category.ParentCategoryId != newParentId;

        if (hasNameChanged || hasParentChanged)
        {
            var newSlug = cleanName.ToSlug();

            var slugExists = await _context.Set<Category>()
                .AnyAsync(c => c.Slug == newSlug && c.ParentCategoryId == newParentId && c.Id != id, 
                    ct);

            if (slugExists)
                throw new AppException(
                    $"A category with the name '{cleanName}' already exists under the target parent category.", 
                    HttpStatusCode.BadRequest);

            category.Name = cleanName;
            category.Slug = newSlug;
        }

        // Step 4: Overwrite all attributes (Full Resource Replacement / PUT)
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;
        category.ParentCategoryId = newParentId;

        await _context.SaveChangesAsync(ct);

        return category.Adapt<CategoryDetailResponse>();
    }
    
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var category = await _context.Set<Category>()
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new AppException($"Category with ID {id} was not found.", HttpStatusCode.NotFound);

        // Guard 1: Prevent soft-deleting if active products are assigned to this category or subcategory
        var hasLinkedProducts = await _context.Set<Product>()
            .AnyAsync(p => p.CategoryId == id || p.SubcategoryId == id, ct);

        if (hasLinkedProducts)
        {
            throw new AppException(
                "Cannot delete category because it is currently assigned to active products.", 
                HttpStatusCode.BadRequest);
        }

        // Guard 2: If this is a Root Category, prevent soft-deleting if active subcategories are linked to it
        if (category.ParentCategoryId == null)
        {
            var hasSubcategories = await _context.Set<Category>()
                .AnyAsync(c => c.ParentCategoryId == id, ct);

            if (hasSubcategories)
            {
                throw new AppException(
                    "Cannot delete category because it has active subcategories linked to it.", 
                    HttpStatusCode.BadRequest);
            }
        }

        // Soft Delete (Handled by AppDbContext SaveChangesAsync interceptor)
        _context.Set<Category>().Remove(category);
        await _context.SaveChangesAsync(ct);
    }
}
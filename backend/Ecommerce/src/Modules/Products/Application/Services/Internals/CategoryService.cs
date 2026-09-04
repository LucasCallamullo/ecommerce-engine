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
using Ecommerce.Shared.Common.Extensions;


/// <summary>
/// Implements domain logic and data persistence operations for catalog categories and subcategories using Mapster.
/// </summary>
public class CategoryService(AppDbContext context) : ICategoryService
{
    private readonly AppDbContext _context = context;

    //* =====================================================================
    //*         METHODS --> GET
    //* =====================================================================

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Only recover Categories Parentss
        return await _context.Set<Category>()
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == null)
            .ProjectToType<CategoryResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .AsNoTracking()
            .Where(c => c.Id == id)
            .ProjectToType<CategoryResponse>()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new AppException($"Category with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<CategoryDetailResponse> GetByIdDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .AsNoTracking()
            .Include(c => c.ParentCategory)
            .Where(c => c.Id == id)
            .ProjectToType<CategoryDetailResponse>()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new AppException($"Category with ID {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<IEnumerable<CategoryResponse>> GetSubcategoriesByParentIdAsync(
        int parentId, CancellationToken cancellationToken = default)
    {
        var parentExists = await _context.Set<Category>()
            .AnyAsync(c => c.Id == parentId, cancellationToken);

        if (!parentExists)
            throw new AppException($"Parent category with ID {parentId} was not found.", HttpStatusCode.NotFound);

        return await _context.Set<Category>()
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == parentId)
            .ProjectToType<CategoryResponse>()
            .ToListAsync(cancellationToken);
    }

    //* =====================================================================
    //*         METHODS --> POST / PUT / DELETE 
    //* =====================================================================

    public async Task<CategoryResponse> CreateAsync(
        CategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        // Step 1: Sanitize Primary Input
        var cleanName = request.Name.Sanitize()
            ?? throw new AppException("Category name cannot be empty.", HttpStatusCode.BadRequest);

        // Step 2: Validate Parent Category Hierarchy (Enforces Max 2-Level Depth)
        if (request.ParentCategoryId.HasValue)
        {
            var parentId = request.ParentCategoryId.Value;

            // Ensure parent exists AND is a root category (cannot link a subcategory to another subcategory)
            var isRootParent = await _context.Set<Category>()
                .AnyAsync(c => c.Id == parentId && c.ParentCategoryId == null, cancellationToken);

            if (!isRootParent)
                throw new AppException(
                    $"Parent category with ID {parentId} was not found or is already a subcategory.", 
                    HttpStatusCode.BadRequest);
        }

        // Step 3: Generate and Validate Unique Slug
        var slug = cleanName.ToSlug();

        var slugExists = await _context.Set<Category>()
            .AnyAsync(c => c.Slug == slug && c.ParentCategoryId == request.ParentCategoryId, cancellationToken);

        if (slugExists)
            throw new AppException(
                $"A category with the name '{cleanName}' already exists.", 
                HttpStatusCode.BadRequest);

        // Step 4: Map Entity with Sanitized Attributes
        var category = new Category
        {
            Name = cleanName,
            Slug = slug,
            Description = request.Description.Sanitize(),
            ImageUrl = request.ImageUrl.Sanitize(),
            IsActive = request.IsActive ?? true,
            ParentCategoryId = request.ParentCategoryId
        };

        // Step 5: Persistence & Response Mapping
        _context.Set<Category>().Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Adapt<CategoryResponse>();
    }

    public async Task<CategoryResponse> UpdateAsync(
        int id, 
        CategoryUpdateRequest request, 
        CancellationToken cancellationToken = default)
    {
        // Step 1: Sanitize Primary Input
        var cleanName = request.Name.Sanitize()
            ?? throw new AppException("Category name cannot be empty.", HttpStatusCode.BadRequest);

        var category = await _context.Set<Category>()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new AppException($"Category with ID {id} was not found.", HttpStatusCode.NotFound);

        var newParentId = request.ParentCategoryId;

        // Step 2: Validate Parent Category Hierarchy Changes
        if (newParentId.HasValue)
        {
            // Rule 1: A Category that was created as a Root Category CANNOT become a Subcategory
            if (category.ParentCategoryId == null)
                throw new AppException(
                    "A root category cannot be converted into a subcategory.", 
                    HttpStatusCode.BadRequest);
            
            // Rule 2: Self-referential Guard: A category cannot be its own parent
            if (newParentId.Value == id)
                throw new AppException("A category cannot be set as its own parent.", HttpStatusCode.BadRequest);

            // Rule 3: If moving a Subcategory to a DIFFERENT parent, validate the new target parent
            if (newParentId != category.ParentCategoryId)
            {
                // Target parent must exist AND must be a Root Category (ParentCategoryId == null)
                var isRootParent = await _context.Set<Category>()
                    .AnyAsync(c => c.Id == newParentId.Value && c.ParentCategoryId == null, cancellationToken);

                if (!isRootParent)
                    throw new AppException(
                        $"Target parent category with ID {newParentId.Value} was not found or is not a root category.", 
                        HttpStatusCode.BadRequest);
            }
        }

        // Step 3: Validate Unique Slug (if Name OR Parent Category changed)
        var hasNameChanged = !string.Equals(category.Name, cleanName, StringComparison.OrdinalIgnoreCase);
        var hasParentChanged = category.ParentCategoryId != newParentId;

        if (hasNameChanged || hasParentChanged)
        {
            var newSlug = cleanName.ToSlug();

            var slugExists = await _context.Set<Category>()
                .AnyAsync(c => c.Slug == newSlug && c.ParentCategoryId == newParentId && c.Id != id, 
                    cancellationToken);

            if (slugExists)
                throw new AppException(
                    $"A category with the name '{cleanName}' already exists under the target parent category.", 
                    HttpStatusCode.BadRequest);

            category.Name = cleanName;
            category.Slug = newSlug;
        }

        // Step 4: Update Mutable Attributes
        category.Description = request.Description.Sanitize();
        category.ImageUrl = request.ImageUrl.Sanitize();
        category.ParentCategoryId = newParentId;

        if (request.IsActive.HasValue)
            category.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return category.Adapt<CategoryResponse>();
    }
    
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Set<Category>()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new AppException($"Category with ID {id} was not found.", HttpStatusCode.NotFound);

        // Guard 1: Prevent soft-deleting if active products are assigned to this category or subcategory
        var hasLinkedProducts = await _context.Set<Product>()
            .AnyAsync(p => p.CategoryId == id || p.SubcategoryId == id, cancellationToken);

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
                .AnyAsync(c => c.ParentCategoryId == id, cancellationToken);

            if (hasSubcategories)
            {
                throw new AppException(
                    "Cannot delete category because it has active subcategories linked to it.", 
                    HttpStatusCode.BadRequest);
            }
        }

        // Soft Delete (Handled by AppDbContext SaveChangesAsync interceptor)
        _context.Set<Category>().Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // =========================================================================
    // PRIVATE HELPERS
    // =========================================================================
}
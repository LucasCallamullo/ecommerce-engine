namespace Ecommerce.Users.Application.Services.Internals;

using Microsoft.EntityFrameworkCore;
using Mapster;
using System.Net;
using Ecommerce.Shared.Auth.Enums;
using Ecommerce.Shared.Database;
using Ecommerce.Shared.Responses;
using Ecommerce.Shared.Exceptions;
using Ecommerce.Shared.Auth.Interfaces;

using Ecommerce.Users.Application.DTOs.Request;
using Ecommerce.Users.Application.DTOs.Response;
using Ecommerce.Users.Application.Interfaces;
using Ecommerce.Users.Domain.Entities;

public class UserService(AppDbContext dbContext, ICurrentUserProvider currentUser) : IUserService
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ICurrentUserProvider _currentUser = currentUser;

    // * ================================================================
    // *            UPDATE METHODS 
    // * ================================================================

    public async Task<UserResponse?> UpdateProfileAsync(
        UpdateUserProfile dto, 
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            throw new AppException("User is not authenticated.", HttpStatusCode.Unauthorized);

        var userId = _currentUser.UserId.Value;

        // 1. Fetch user entity for tracking to perform partial update.
        var user = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken) ??
            throw new AppException($"User with Id: {userId} Not found", HttpStatusCode.NotFound);

        // 2. Mutate properties only if non-empty values are provided in request.
        if (!string.IsNullOrWhiteSpace(dto.FirstName))
            user.FirstName = dto.FirstName;

        if (!string.IsNullOrWhiteSpace(dto.LastName))
            user.LastName = dto.LastName;

        if (dto.Cellphone != null)
            user.Cellphone = dto.Cellphone;

        if (dto.Dni != null)
            user.Dni = dto.Dni;

        // 3. Update audit timestamp and save tracked changes.
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 4. Re-query fresh projection to return updated DTO.
        return await GetByIdAsync(userId, cancellationToken);
    }

    public async Task<UserResponse?> UpdateUserRoleAsync(
        UpdateUserRol dto, 
        CancellationToken cancellationToken = default)
    {
        // 1. Fetch target user entity along with existing role relational mappings.
        var emailFilter = dto.Email.Trim().ToLower();

        var user = await _dbContext.Set<User>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(
                u => u.Email.ToLower() == emailFilter && !u.IsDeleted, 
                cancellationToken) ??
            throw new AppException($"User with Email: {dto.Email} Not found", HttpStatusCode.NotFound);

        // 2. Parse requested string role names to corresponding enum/ID values.
        var requestedRoleIds = dto.Roles
            .Select(r => Enum.TryParse<UserRoleEnum>(r, true, out var parsedRole) ? (int)parsedRole : (int?)null)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .ToHashSet();

        // 3. Identify and purge role relationships no longer present in payload.
        var rolesToRemove = user.UserRoles
            .Where(ur => !requestedRoleIds.Contains(ur.RoleId))
            .ToList();

        foreach (var role in rolesToRemove)
        {
            user.UserRoles.Remove(role);
        }

        // 4. Identify and append newly assigned role relationships.
        var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();
        var rolesToAdd = requestedRoleIds
            .Where(id => !existingRoleIds.Contains(id))
            .Select(id => new UserRole { UserId = user.Id, RoleId = id });

        foreach (var newRole in rolesToAdd)
        {
            user.UserRoles.Add(newRole);
        }

        // 5. Update audit timestamp and commit role changes.
        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 6. Return projected updated user profile with refreshed roles.
        return await GetByIdAsync(user.Id, cancellationToken);
    }

    // * ================================================================
    // *            GET METHODS 
    // * ================================================================

    public async Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 1. Direct LINQ projection from database entity to response DTO bypassing EF Core tracking.
        return await _dbContext.Set<User>()
            .AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .ProjectToType<UserResponse>()
            .FirstOrDefaultAsync(cancellationToken) ??
            throw new AppException($"User with Id: {userId} Not found", HttpStatusCode.NotFound);
    }

    public async Task<PagedResultDto<UserResponse>> GetAllAsync(
        UserFilterParams filterParams, 
        CancellationToken cancellationToken = default)
    {
        // 1. Build base query excluding soft-deleted records with tracking disabled for performance.
        var query = _dbContext.Set<User>()
            .AsNoTracking()
            .Where(u => !u.IsDeleted);

        // 2. Apply optional email filtering with case-insensitive matching.
        if (!string.IsNullOrWhiteSpace(filterParams.Email))
        {
            var emailFilter = filterParams.Email.ToLower().Trim();
            query = query.Where(u => u.Email.ToLower().Contains(emailFilter));
        }

        // 3. Apply optional multi-field search term filtering (First name, Last name, Email, DNI).
        if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
        {
            var term = filterParams.SearchTerm.ToLower().Trim();
            query = query.Where(u => 
                u.FirstName.ToLower().Contains(term) || 
                u.LastName.ToLower().Contains(term) || 
                u.Email.ToLower().Contains(term) ||
                (u.Dni != null && u.Dni.Contains(term)));
        }

        // 4. Apply optional account status filter.
        if (filterParams.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == filterParams.IsActive.Value);
        }

        // 5. Execute count query to calculate total matching records before pagination.
        var totalCount = await query.CountAsync(cancellationToken);

        // 6. Project paginated slice directly into UserResponse collection.
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
            .Take(filterParams.PageSize)
            .ProjectToType<UserResponse>()
            .ToListAsync(cancellationToken);

        // 7. Return wrapped response containing result items and pagination metadata.
        return new PagedResultDto<UserResponse>(items, totalCount, filterParams.PageNumber, filterParams.PageSize);
    }
}
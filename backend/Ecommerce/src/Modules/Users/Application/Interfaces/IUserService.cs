namespace Ecommerce.Users.Application.Interfaces;

using Ecommerce.Shared.Responses;
using Ecommerce.Users.Application.DTOs.Request;
using Ecommerce.Users.Application.DTOs.Response;

/// <summary>
/// Defines business operation contracts for user profile management, administrative role assignments, and paginated searches.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a detailed user profile by its unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to retrieve.</param>
    Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of user profiles based on dynamic filtering criteria.
    /// </summary>
    /// <param name="filterParams">The query parameters used to filter, search, and paginate user records.</param>
    Task<PagedResultDto<UserResponse>> GetAllAsync(UserFilterParams filterParams, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a partial update on the self-service profile information for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the authenticated user performing the update.</param>
    /// <param name="dto">The payload containing updated personal details.</param>
    Task<UserResponse?> UpdateProfileAsync(UpdateUserProfile dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates assigned security roles for a target user account. Restricted to administrative users.
    /// </summary>
    /// <param name="dto">The administrative payload containing the target user's email and the new collection of assigned role names.</param>
    Task<UserResponse?> UpdateUserRoleAsync(UpdateUserRol dto, CancellationToken cancellationToken = default);
}
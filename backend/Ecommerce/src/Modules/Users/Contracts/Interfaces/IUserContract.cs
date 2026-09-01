namespace Ecommerce.Users.Contracts.Interfaces;

using Ecommerce.Users.Contracts.DTOs;

/// <summary>
/// Contract interface exposed by the Users module to handle identity queries 
/// and user provisioning operations for external modules (e.g., Auth).
/// </summary>
public interface IUserContract
{
    /// <summary>
    /// Retrieves authentication and security profile details for a given user email address.
    /// </summary>
    /// <param name="email">The unique email address of the user to look up.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The user's security profile if found; otherwise, <c>null</c>.</returns>
    Task<UserAuthDetailsDto?> GetAuthDetailsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves authentication and security profile details by user identifier.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the user.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The user's security profile if found; otherwise, <c>null</c>.</returns>
    Task<UserAuthDetailsDto?> GetAuthDetailsByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves detailed user profile information for profile management views.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the target user.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The detailed user profile DTO if found; otherwise, <c>null</c>.</returns>
    Task<UserProfileDto?> GetUserProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Provisions a new user account with default roles in the system.
    /// </summary>
    /// <param name="request">The user creation payload containing basic profile and hashed password data.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A summary snapshot of the newly created user.</returns>
    Task<UserCreatedDto> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken = default);
}
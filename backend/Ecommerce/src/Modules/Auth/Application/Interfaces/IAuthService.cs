using Ecommerce.Auth.Application.DTOs.Request;
using Ecommerce.Auth.Application.DTOs.Response;

namespace Ecommerce.Auth.Application.Interfaces;

/// <summary>
/// Defines business operations for user authentication, registration, token issuance, and session management.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user using their credentials and issues fresh access and refresh tokens.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>An <see cref="AuthResponse"/> containing tokens and profile summary.</returns>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user account and generates initial authentication tokens.
    /// </summary>
    /// <param name="request">The user registration details.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>An <see cref="AuthResponse"/> containing tokens and profile summary.</returns>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a refresh token and issues a new pair of access and refresh tokens (token rotation).
    /// </summary>
    /// <param name="request">The refresh token payload.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>An <see cref="AuthResponse"/> containing the newly issued tokens.</returns>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves full profile details for the specified user identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user extracted from claims.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A <see cref="UserProfileResponse"/> containing personal profile data and assigned roles.</returns>
    Task<UserProfileResponse> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an active refresh token, effectively logging out the user from that session.
    /// </summary>
    /// <param name="request">The payload containing the refresh token to revoke.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);
}
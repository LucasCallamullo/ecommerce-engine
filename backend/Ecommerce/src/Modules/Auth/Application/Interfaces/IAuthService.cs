using Ecommerce.Auth.Application.DTOs.Request;
using Ecommerce.Auth.Application.DTOs.Response;

namespace Ecommerce.Auth.Application.Interfaces;

/// <summary>
/// Defines business operations for user authentication, registration, and token issuance.
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
}
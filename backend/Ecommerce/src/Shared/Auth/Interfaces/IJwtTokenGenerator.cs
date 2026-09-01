namespace Ecommerce.Shared.Auth.Interfaces;

/// <summary>
/// Defines the contract for generating and validating cryptographically signed Access Tokens (JWT) 
/// and Refresh Tokens for client authentication.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a new pair of access and refresh tokens along with token expiration metadata.
    /// </summary>
    /// <param name="userId">The unique identifier (GUID) of the authenticated user.</param>
    /// <param name="email">The email address of the authenticated user.</param>
    /// <param name="roles">A collection of security roles assigned to the user.</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item><description><c>AccessToken</c>: The cryptographically signed JWT bearer token.</description></item>
    /// <item><description><c>RefreshToken</c>: The cryptographically signed JWT refresh token.</description></item>
    /// <item><description><c>ExpiresAt</c>: The UTC timestamp indicating when the access token expires.</description></item>
    /// </list>
    /// </returns>
    (string AccessToken, string RefreshToken, DateTime ExpiresAt) GenerateTokens(
        Guid userId, 
        string email, 
        IEnumerable<string> roles);

    /// <summary>
    /// Validates a refresh token's signature, purpose, and expiration, extracting the associated user identifier.
    /// </summary>
    /// <param name="refreshToken">The raw refresh token string to validate.</param>
    /// <returns>The extracted user <see cref="Guid"/> if valid; otherwise, <see cref="Guid.Empty"/>.</returns>
    Guid ValidateRefreshToken(string refreshToken);
}
namespace Ecommerce.Shared.Auth.Interfaces;

/// <summary>
/// Contract for generating signed JSON Web Tokens (JWT) containing user identity claims.
/// Implemented by identity services to produce access tokens upon authentication.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a signed JWT string populated with the provided user context and role claims.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="roles">The list of security roles assigned to the user.</param>
    /// <returns>A cryptographically signed JWT bearer token string.</returns>
    string GenerateToken(Guid userId, string email, IEnumerable<string> roles);
}
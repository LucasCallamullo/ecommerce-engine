namespace Ecommerce.Auth.Application.DTOs.Response;

/// <summary>
/// Data transfer object representing the successful authentication response,
/// containing security tokens and core identity claims for client application state.
/// </summary>
/// <param name="UserId">The unique identifier of the authenticated user.</param>
/// <param name="Email">The user's primary email address.</param>
/// <param name="FirstName">The first name of the user for UI display purposes.</param>
/// <param name="LastName">The last name of the user for UI display purposes.</param>
/// <param name="AccessToken">The short-lived cryptographically signed JSON Web Token (JWT) bearer string.</param>
/// <param name="RefreshToken">The long-lived token string used to request new access tokens without re-authenticating.</param>
/// <param name="ExpiresAt">The exact UTC timestamp indicating when the access token expires.</param>
/// <param name="Roles">The collection of assigned role names (e.g., "Customer", "Admin").</param>
public record AuthResponse(
    Guid UserId, 
    string Email, 
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    IEnumerable<string> Roles
);
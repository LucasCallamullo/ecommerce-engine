namespace Ecommerce.Auth.Application.DTOs.Request;

/// <summary>
/// Data transfer object carrying required credentials for user authentication.
/// </summary>
/// <param name="Email">The unique email address associated with the user account.</param>
/// <param name="Password">The plain-text password to be verified during authentication.</param>
public record LoginRequest(
    string Email, 
    string Password
);
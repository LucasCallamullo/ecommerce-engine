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


/// <summary>
/// Data transfer object carrying user registration details for new account creation.
/// </summary>
/// <param name="Email">The unique email address used as the primary login credential.</param>
/// <param name="Password">The plain-text password to be hashed and stored upon registration.</param>
/// <param name="FirstName">The first name of the registering user.</param>
/// <param name="LastName">The last name of the registering user.</param>
/// <param name="Cellphone">An optional primary contact phone number.</param>
/// <param name="Dni">An optional National Identity Document (DNI) number.</param>
public record RegisterRequest(
    string Email, 
    string Password, 
    string FirstName, 
    string LastName, 
    string? Cellphone, 
    string? Dni
);


/// <summary>
/// Data transfer object representing the payload required to explicitly revoke a refresh token and terminate a user session.
/// </summary>
/// <param name="RefreshToken">The refresh token to be revoked.</param>
public record LogoutRequest(
    string RefreshToken
);


/// <summary>
/// Data transfer object representing the payload required to renew an expired access token using a refresh token.
/// </summary>
/// <param name="RefreshToken">The active refresh token issued during login or previous refresh operation.</param>
public record RefreshTokenRequest(
    string RefreshToken
);
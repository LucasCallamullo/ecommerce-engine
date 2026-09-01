namespace Ecommerce.Auth.Application.DTOs.Response;

/// <summary>
/// Core identity profile details returned to client applications upon successful authentication.
/// </summary>
/// <param name="Id">The unique identifier of the authenticated user.</param>
/// <param name="Email">The user's primary email address.</param>
/// <param name="FirstName">The first name of the user for UI display purposes.</param>
/// <param name="LastName">The last name of the user for UI display purposes.</param>
/// <param name="Cellphone">Primary contact phone number, if available.</param>
/// <param name="Dni">National identification number, if available.</param>
/// <param name="Roles">The collection of assigned role names (e.g., "Customer", "Admin").</param>
public record UserAuthResponse(
    Guid Id, 
    string Email, 
    string FirstName,
    string LastName,
    string? Cellphone,
    string? Dni,
    IEnumerable<string> Roles
);

/// <summary>
/// Data transfer object representing the successful authentication response,
/// containing security tokens and core identity claims for client application state.
/// </summary>
public record AuthResponse(
    UserAuthResponse User,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
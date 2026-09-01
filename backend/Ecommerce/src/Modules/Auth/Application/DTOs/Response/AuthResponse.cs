namespace Ecommerce.Auth.Application.DTOs.Response;

/// <summary>
/// Core identity profile details returned to client applications upon successful authentication.
/// </summary>
/// <param name="Id">The unique primary key identifier of the authenticated user.</param>
/// <param name="Email">The user's primary registered email address.</param>
/// <param name="FirstName">The first name of the user for UI display purposes.</param>
/// <param name="LastName">The last name of the user for UI display purposes.</param>
/// <param name="Cellphone">The optional primary contact phone number.</param>
/// <param name="Dni">The optional national identification document number (DNI).</param>
/// <param name="Roles">The collection of assigned security role names (e.g., "Customer", "Admin").</param>
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
/// Data transfer object representing the successful authentication response, containing security tokens and core identity details.
/// </summary>
/// <param name="User">The authenticated user's profile summary and security claims.</param>
/// <param name="AccessToken">The short-lived JWT access token used to authorize API requests.</param>
/// <param name="RefreshToken">The long-lived refresh token used to obtain new access tokens upon expiration.</param>
/// <param name="ExpiresAt">The UTC timestamp indicating when the access token expires.</param>
public record AuthResponse(
    UserAuthResponse User,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);


/// <summary>
/// Data transfer object representing the detailed profile information of the currently authenticated user.
/// </summary>
/// <param name="Id">The unique primary key identifier of the user account.</param>
/// <param name="Email">The registered unique email address of the user.</param>
/// <param name="FirstName">The first name of the user.</param>
/// <param name="LastName">The last name of the user.</param>
/// <param name="Cellphone">The optional contact telephone or mobile number.</param>
/// <param name="Dni">The optional national identity document number (DNI).</param>
/// <param name="IsActive">Indicates whether the user account is currently enabled in the system.</param>
/// <param name="Roles">The collection of security roles assigned to the user (e.g., "Customer", "Admin").</param>
/// <param name="UpdatedAt">The optional UTC timestamp indicating when the user profile was last updated.</param>
/// <param name="CreatedAt">The UTC timestamp indicating when the user account was created.</param>
public record UserProfileResponse(
    Guid Id, 
    string Email, 
    string FirstName,
    string LastName,
    string? Cellphone,
    string? Dni,
    bool IsActive,
    IEnumerable<string> Roles,
    DateTime? UpdatedAt,
    DateTime CreatedAt
);
namespace Ecommerce.Users.Contracts.DTOs;

//? ========================================================
//?                  REQUEST DTOs
//? ========================================================

/// <summary>
/// Data required by external modules to create a new user record.
/// </summary>
/// <param name="Email">The unique email address for authentication.</param>
/// <param name="PasswordHash">The pre-hashed password string from the Auth module.</param>
/// <param name="FirstName">User's given name.</param>
/// <param name="LastName">User's surname.</param>
/// <param name="Cellphone">Primary contact phone number.</param>
/// <param name="Dni">National identification number.</param>
public record CreateUserDto(
    string Email,
    string PasswordHash,
    string FirstName,
    string LastName,
    string? Cellphone,
    string? Dni
);

//? ========================================================
//?                  RESPONSE DTOs
//? ========================================================

/// <summary>
/// Contains complete security and profile data required by the Auth module during login validation.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Email">The user's primary email address.</param>
/// <param name="PasswordHash">The stored password hash to verify against input credentials.</param>
/// <param name="FirstName">User's given name.</param>
/// <param name="LastName">User's surname.</param>
/// <param name="Cellphone">Primary contact phone number.</param>
/// <param name="Dni">National identification number.</param>
/// <param name="IsActive">Indicates whether the account is active and allowed to log in.</param>
/// <param name="Roles">Collection of assigned system role names.</param>
public record UserAuthDetailsDto(
    Guid Id,
    string Email,
    string PasswordHash,
    string FirstName,
    string LastName,
    string? Cellphone,
    string? Dni,
    bool IsActive,
    IReadOnlyCollection<string> Roles
);


/// <summary>
/// Represents the identity snapshot returned after successfully registering a new user.
/// </summary>
/// <param name="Id">The unique identifier of the created user.</param>
/// <param name="Email">The user's primary email address.</param>
/// <param name="FirstName">User's given name.</param>
/// <param name="LastName">User's surname.</param>
/// <param name="Cellphone">Primary contact phone number.</param>
/// <param name="Dni">National identification number.</param>
/// <param name="Roles">Collection of assigned system role names (e.g., "Customer").</param>
public record UserCreatedDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Cellphone,
    string? Dni,
    IReadOnlyCollection<string> Roles
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
public record UserProfileDto(
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
namespace Ecommerce.Users.Application.DTOs.Response;


/// <summary>
/// Represents a comprehensive data transfer object containing user profile details and role assignments.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Email">The primary email address associated with the account.</param>
/// <param name="FirstName">The user's given or first name.</param>
/// <param name="LastName">The user's family or last name.</param>
/// <param name="Cellphone">The optional contact phone number for the user.</param>
/// <param name="Dni">The optional national identification document number.</param>
/// <param name="IsActive">Indicates whether the user account is active and permitted to access the system.</param>
/// <param name="Roles">The collection of role names assigned to the user account.</param>
/// <param name="UpdatedAt">The UTC timestamp when the user profile was last modified, if applicable.</param>
/// <param name="CreatedAt">The UTC timestamp when the user account was initially created.</param>
public record UserResponse(
    Guid Id, 
    string Email, 
    string FirstName,
    string LastName,
    string? Cellphone,
    string? Dni,
    bool IsActive,
    IReadOnlyCollection<string> Roles,
    DateTime? UpdatedAt,
    DateTime CreatedAt
);
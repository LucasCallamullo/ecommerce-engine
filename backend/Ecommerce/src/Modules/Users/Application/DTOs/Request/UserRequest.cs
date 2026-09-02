namespace Ecommerce.Users.Application.DTOs.Request;


/// <summary>
/// Represents the payload for updating an authenticated user's personal profile details.
/// Supports partial updates; omitted or null properties are preserved with their existing values.
/// </summary>
/// <param name="FirstName">The optional updated given or first name.</param>
/// <param name="LastName">The optional updated family or last name.</param>
/// <param name="Cellphone">The optional updated contact phone number.</param>
/// <param name="Dni">The optional updated national identification document number.</param>
public record UpdateUserProfile(
    string? FirstName = null,
    string? LastName = null,
    string? Cellphone = null,
    string? Dni = null
);


/// <summary>
/// Represents the administrative payload for modifying role assignments associated with a target user account.
/// Restrict execution exclusively to system administrators.
/// </summary>
/// <param name="Email">The target user's email address whose assigned roles are being updated.</param>
/// <param name="Roles">The complete collection of role names to assign to the target user account.</param>
public record UpdateUserRol(
    string Email,
    List<string> Roles
);


/// <summary>
/// Encapsulates query parameters and pagination controls for filtering and searching user records.
/// </summary>
/// <param name="SearchTerm">Optional string to search across first name, last name, email, or identification attributes.</param>
/// <param name="Email">Optional strict or partial filter targeting the email address attribute.</param>
/// <param name="IsActive">Optional state filter to restrict results by account active status.</param>
/// <param name="PageNumber">The target page index for the paginated result set (1-based index, defaults to 1).</param>
/// <param name="PageSize">The maximum number of user records to return per page (defaults to 10).</param>
public record UserFilterParams(
    string? SearchTerm,
    string? Email,
    bool? IsActive,
    int PageNumber = 1,
    int PageSize = 10
);
using Ecommerce.Shared.Common;

namespace Ecommerce.Users.Domain.Entities;

/// <summary>
/// Represents an application user within the authentication and user management domain.
/// 
/// Inherits from <see cref="BaseEntity{T}"/> to provide core auditing attributes:
/// - Id (int, Primary Key) 
/// - CreatedAt (DateTime, UTC timestamp upon insertion)
/// - UpdatedAt (DateTime?, nullable UTC timestamp upon modification)
/// - IsDeleted (bool, soft delete logical flag)
/// </summary>
public class User : BaseEntity<Guid>
{
    /// <summary>Gets or sets the unique email address used as the primary login credential.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's primary contact phone number.</summary>
    public string? Cellphone { get; set; }

    /// <summary>Gets or sets the National Identity Document (DNI) number.</summary>
    public string? Dni { get; set; }

    /// <summary>Gets or sets the salted and hashed password.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name of the user.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the last name of the user.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the account is active and allowed to authenticate.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the exact UTC timestamp when the user account was soft deleted.</summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>Gets or sets the collection of roles assigned to this user (M:N).</summary>
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
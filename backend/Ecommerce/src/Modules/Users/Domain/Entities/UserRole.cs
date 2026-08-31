using Ecommerce.Shared.Common;

namespace Ecommerce.Users.Domain.Entities;

/// <summary>
/// Represents a security role within the authorization system.
/// 
/// Inherits from <see cref="BaseEntity{T}"/> to provide core auditing attributes:
/// - Id (int, Primary Key) 
/// - CreatedAt (DateTime, UTC timestamp upon insertion)
/// - UpdatedAt (DateTime?, nullable UTC timestamp upon modification)
/// - IsDeleted (bool, soft delete logical flag)
/// </summary>
public class Role : BaseEntity<int>
{
    /// <summary>Gets or sets the unique name of the role (e.g., "Admin", "Customer").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets an optional description of the permissions granted by this role.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets the collection of user-role join entities assigned to this role (M:N).</summary>
    public ICollection<UserRole> UserRoles { get; set; } = [];
}

/// <summary>Represents the join entity for the many-to-many relationship between users and roles.</summary>
public class UserRole
{
    /// <summary>Gets or sets the foreign key for the associated user.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the foreign key for the associated role.</summary>
    public int RoleId { get; set; }


    /// <summary>Gets or sets the navigation property for the associated user.</summary>
    public User User { get; set; } = null!;

    /// <summary>Gets or sets the navigation property for the associated role.</summary>
    public Role Role { get; set; } = null!;
}
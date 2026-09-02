using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Ecommerce.Users.Domain.Entities;

namespace Ecommerce.Users.Infrastructure.Configurations;

/// <summary>
/// Configures the Entity Framework Core mapping for the <see cref="UserRole"/> join entity.
/// Defines the composite primary key and foreign key relationships between users and roles.
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Define composite primary key (UserId + RoleId)
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // Configure User relationship (Many-to-One)
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        // Configure Role relationship (Many-to-One)
        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
    }
}
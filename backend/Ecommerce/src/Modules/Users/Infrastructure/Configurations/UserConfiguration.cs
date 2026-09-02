using Ecommerce.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Users.Infrastructure.Configurations;

/// <summary>
/// Configures Entity Framework Core ORM mapping, constraints, indexes,
/// and soft-delete filters for the <see cref="User"/> entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Explicit Primary Key configuration
        builder.HasKey(u => u.Id);

        // Text properties constraints
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Cellphone)
            .HasMaxLength(30);

        builder.Property(u => u.Dni)
            .HasMaxLength(20);

        // Unique index on Email to prevent duplicate account registration
        builder.HasIndex(u => u.Email)
            .IsUnique();

        // Optional non-unique index on DNI for faster lookup during identification verification
        // builder.HasIndex(u => u.Dni);

        // Global Query Filter for Soft Delete handling
        // builder.HasQueryFilter(u => !u.IsDeleted);

        // Configure Many-to-Many relationship with Roles via UserRole join entity
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
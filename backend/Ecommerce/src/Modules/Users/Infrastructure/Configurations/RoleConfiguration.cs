namespace Ecommerce.Users.Infrastructure.Configurations;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


using Ecommerce.Shared.Auth.Enums;
using Ecommerce.Shared.Common.Extensions;
using Ecommerce.Users.Domain.Entities;


/// <summary>
/// Configures Entity Framework Core ORM mapping, constraints, indexes, 
/// and initial seed data for the <see cref="Role"/> entity.
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Explicit primary key configuration inherited from BaseEntity
        builder.HasKey(r => r.Id);

        // Name field constraints
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(30);

        // Optional description field constraint
        builder.Property(r => r.Description)
            .HasMaxLength(200);

        // Enforces unique constraint at the database level via a unique index,
        // preventing duplicate role names from being inserted.
        builder.HasIndex(r => r.Name)
            .IsUnique();
    }
}
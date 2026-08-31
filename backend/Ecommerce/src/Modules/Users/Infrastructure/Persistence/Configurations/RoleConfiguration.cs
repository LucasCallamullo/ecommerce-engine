using Ecommerce.Shared.Common.Enums;
using Ecommerce.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Users.Infrastructure.Persistence.Configurations;

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

        // Seed initial system roles during EF Core database migrations 
        // using strongly-typed values from UserRoleEnum.
        builder.HasData(
            new Role 
            { 
                Id = (int)UserRoleEnum.Admin, 
                Name = nameof(UserRoleEnum.Admin), 
                Description = UserRoleEnum.Admin.GetDescription() 
            },
            new Role 
            { 
                Id = (int)UserRoleEnum.Customer, 
                Name = nameof(UserRoleEnum.Customer), 
                Description = UserRoleEnum.Customer.GetDescription() 
            },
            new Role 
            { 
                Id = (int)UserRoleEnum.Seller, 
                Name = nameof(UserRoleEnum.Seller), 
                Description = UserRoleEnum.Seller.GetDescription() 
            },
            new Role 
            { 
                Id = (int)UserRoleEnum.Support, 
                Name = nameof(UserRoleEnum.Support), 
                Description = UserRoleEnum.Support.GetDescription() 
            }
        );
    }
}
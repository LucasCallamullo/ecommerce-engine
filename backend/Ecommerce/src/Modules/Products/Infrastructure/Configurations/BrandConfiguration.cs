namespace Ecommerce.Products.Infrastructure.Configurations;

using Ecommerce.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity Framework Core Fluent API configuration for mapping the <see cref="Brand"/> domain entity 
/// to the underlying database schema.
/// </summary>
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    /// <summary>
    /// Configures the database schema mappings, property constraints, and index definitions for the <see cref="Brand"/> entity.
    /// </summary>
    /// <param name="builder">The builder providing the API to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        // 1. Table & Primary Key Mapping
        builder.ToTable("products_brands");

        // Primary key configuration (automatically creates a clustered index by default)
        builder.HasKey(b => b.Id);

        // 2. Property Column Constraints & Requirements
        builder.Property(b => b.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(b => b.Slug)
            .HasMaxLength(69)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(100);

        builder.Property(b => b.ImageUrl)
            .HasMaxLength(200);

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION
        // -------------------------------------------------------------

        // Dynamically retrieve the mapped database column name for IsDeleted
        // to guarantee compile-time type safety across database conventions (e.g., snake_case).
        var isDeletedColumn = builder.Property(b => b.IsDeleted)
            .Metadata.GetColumnName();

        // Filtered unique index enforcing slug uniqueness exclusively for active (non-deleted) brands.
        // Allows slug recycling if a brand was previously soft-deleted.
        builder.HasIndex(b => b.Slug)
            .HasFilter($"{isDeletedColumn} = 0")
            .IsUnique();
    }
}
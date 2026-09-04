namespace Ecommerce.Products.Infrastructure.Configurations;

using Ecommerce.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity Framework Core Fluent API configuration for mapping the <see cref="Product"/> domain entity 
/// to the underlying database schema.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Configures the database schema mappings, property constraints, indexes, 
    /// and relationships for the <see cref="Product"/> entity.
    /// </summary>
    /// <param name="builder">The builder providing the API to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // 1. Table & Primary Key Mapping
        builder.ToTable("products_products");

        // Primary key configuration (automatically creates a clustered index by default)
        builder.HasKey(p => p.Id);

        // 2. Property Column Constraints & Requirements
        builder.Property(p => p.Name)
            .HasMaxLength(140)
            .IsRequired();

        builder.Property(p => p.Slug)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.MainImageUrl)
            .HasMaxLength(220);

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION
        // -------------------------------------------------------------

        // Dynamically retrieve the mapped database column name for IsDeleted
        // to guarantee compile-time type safety across database conventions (e.g., snake_case).
        var isDeletedColumn = builder.Property(p => p.IsDeleted)
            .Metadata.GetColumnName()!;

        // Filtered unique index enforcing slug uniqueness exclusively for non-deleted products.
        // Allows slug recycling if a product was previously soft-deleted.
        builder.HasIndex(p => p.Slug)
            .HasFilter($"{isDeletedColumn} = 0")
            .IsUnique();

        // Composite Indexes: Optimizes public catalog filtering by Category/Brand combined with active status
        builder.HasIndex(p => new { p.CategoryId, p.IsActive });
        builder.HasIndex(p => new { p.BrandId, p.IsActive });

        // Composite Index for Subcategory: Handles catalog searches within nested subcategories
        builder.HasIndex(p => new { p.SubcategoryId, p.IsActive });

        // -------------------------------------------------------------
        // RELATIONSHIPS CONFIGURATION
        // -------------------------------------------------------------

        // Primary Category Relationship (Optional foreign key)
        builder.HasOne(p => p.Category)
            .WithMany(c => c.PrimaryProducts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Subcategory Relationship (Optional foreign key)
        builder.HasOne(p => p.Subcategory)
            .WithMany(c => c.SubcategoryProducts)
            .HasForeignKey(p => p.SubcategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Brand Relationship (Optional foreign key)
        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
namespace Ecommerce.Products.Infrastructure.Configurations;

using Ecommerce.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity Framework Core configuration for mapping the <see cref="Category"/> domain entity to the database schema.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // 1. Table & Primary Key Mapping
        builder.ToTable("products_categories");

        builder.HasKey(c => c.Id);

        // 2. Property Length Constraints & Requirements
        builder.Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Slug)
            .HasMaxLength(69)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(100);

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(200);

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION
        // -------------------------------------------------------------

        // Filtered unique index allowing slug reuse after soft deletion
        // Get the exact database column name mapped for IsDeleted
        var isDeletedColumn = builder.Property(c => c.IsDeleted)
            .Metadata.GetColumnName();

        builder.HasIndex(c => c.Slug)
            .HasFilter($"{isDeletedColumn} = 0")
            .IsUnique();

        // Composite Index: Highly effective for fetching active subcategories under a specific parent node
        builder.HasIndex(c => new { c.ParentCategoryId, c.IsActive });

        // -------------------------------------------------------------
        // SELF-REFERENTIAL RELATIONSHIP CONFIGURATION
        // -------------------------------------------------------------

        builder.HasOne(c => c.ParentCategory)
            .WithMany(pc => pc.Subcategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            // Restrict prevents deleting a parent category while child subcategories are linked to it
    }
}
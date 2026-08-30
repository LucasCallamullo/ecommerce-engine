using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Products.Domain.Entities;

namespace Ecommerce.Products.Infrastructure.Persistence.Configurations;

// Fluent API configuration for Category entity (maps rules to EF Core).
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("products_categories");

        // PK - Automatically creates a Clustered Index (does not require HasIndex)
        builder.HasKey(b => b.Id);

        // Name constraints
        builder.Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();

        // Slug constraints & Unique Index
        builder.Property(c => c.Slug)
            .HasMaxLength(69)
            .IsRequired();

        // Optional fields string lengths
        builder.Property(c => c.Description)
            .HasMaxLength(100);

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(200);

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION
        // -------------------------------------------------------------
        builder.HasIndex(c => c.Slug)
            .IsUnique();

        builder.HasIndex(b => b.IsActive);

        // Composite Index: Ideal for queries filtering top-level active categories
        builder.HasIndex(c => new { c.ParentCategoryId, c.IsActive });

        // -------------------------------------------------------------
        // SELF-REFERENTIAL RELATIONSHIP CONFIGURATION
        // -------------------------------------------------------------

        builder.HasOne(c => c.ParentCategory)
            .WithMany(pc => pc.Subcategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict); 
            // Restrict prevents deleting a parent category if it still has active subcategories
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductEntity = Ecommerce.Products.Domain.Entities.Product;

namespace Ecommerce.Products.Infrastructure.Persistence.Configurations;

// Fluent API configuration for Product entity (maps rules to EF Core).
public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        // Enforces table name explicitly
        builder.ToTable("products_products");

        // PK - Automatically creates a Clustered Index (does not require HasIndex)
        builder.HasKey(p => p.Id);

        // Name constraints
        builder.Property(p => p.Name)
            .HasMaxLength(140)
            .IsRequired();

        // Slug constraints
        builder.Property(p => p.Slug)
            .HasMaxLength(160)
            .IsRequired();

        // Optional fields string length limits
        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.MainImage)
            .HasMaxLength(220);

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION
        // -------------------------------------------------------------

        // Unique Index for URL routing
        builder.HasIndex(p => p.Slug)
            .IsUnique();

        // Composite Indexes: Cover searches by Category/Brand ALONE and Category/Brand + IsActive
        builder.HasIndex(p => new { p.CategoryId, p.IsActive });
        builder.HasIndex(p => new { p.BrandId, p.IsActive });

        // Single Index for Subcategory (only if you don't make a composite index for subcategory)
        builder.HasIndex(p => p.SubcategoryId);

        builder.HasIndex(p => p.IsActive);

        // -------------------------------------------------------------
        // RELATIONSHIPS CONFIGURATION
        // -------------------------------------------------------------

        // Primary Category Relationship (Optional)
        builder.HasOne(p => p.Category)
            .WithMany(c => c.PrimaryProducts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Subcategory Relationship (Optional)
        builder.HasOne(p => p.Subcategory)
            .WithMany(c => c.SubcategoryProducts)
            .HasForeignKey(p => p.SubcategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Brand Relationship (Optional)
        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
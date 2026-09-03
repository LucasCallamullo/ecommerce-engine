namespace Ecommerce.Products.Infrastructure.Configurations;

using Ecommerce.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity Framework Core Fluent API configuration for mapping the <see cref="ProductImage"/> domain entity 
/// to the underlying database schema.
/// </summary>
public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    /// <summary>
    /// Configures the database schema mappings, property constraints, composite indexes, and relationships for the <see cref="ProductImage"/> entity.
    /// </summary>
    /// <param name="builder">The builder providing the API to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        // 1. Table & Primary Key Mapping
        builder.ToTable("products_images");

        // Primary key configuration (automatically creates a clustered index by default)
        builder.HasKey(p => p.Id);

        // 2. Property Column Constraints & Requirements
        builder.Property(i => i.Url)
            .HasMaxLength(220)
            .IsRequired();

        // Optional AltText mapping (ready for activation if uncommented in the domain entity)
        builder.Property(i => i.AltText)
            .HasMaxLength(200);

        builder.Property(i => i.DisplayOrder)
            .HasDefaultValue(0);

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION & GALLERY SORTING
        // -------------------------------------------------------------

        // Composite Index: Optimizes fetching master product image galleries pre-sorted by display order
        builder.HasIndex(i => new { i.ProductId, i.DisplayOrder });

        // Composite Index: Optimizes fetching specific product variant image galleries pre-sorted by display order
        builder.HasIndex(i => new { i.ProductVariantId, i.DisplayOrder });

        // -------------------------------------------------------------
        // RELATIONSHIPS CONFIGURATION
        // -------------------------------------------------------------

        // Master Product Relationship (Mandatory foreign key)
        builder.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product Variant Relationship (Optional foreign key)
        builder.HasOne(i => i.ProductVariant)
            .WithMany(pv => pv.Images)
            .HasForeignKey(i => i.ProductVariantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Products.Domain.Entities;

namespace Ecommerce.Products.Infrastructure.Persistence.Configurations;

// Fluent API configuration for ProductImage entity (maps rules to EF Core).
public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("products_images");

        // PK - Automatically creates a Clustered Index (does not require HasIndex)
        builder.HasKey(p => p.Id);

        // URL constraints (Required, Max length 1000)
        builder.Property(i => i.Url)
            .HasMaxLength(220)
            .IsRequired();

        // Optional AltText (Commented out in domain, keep ready if uncommented)
        // builder.Property(i => i.AltText)
        //     .HasMaxLength(255);

        // DisplayOrder default value
        builder.Property(i => i.DisplayOrder)
            .HasDefaultValue(0);

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION & GALLERY SORTING
        // -------------------------------------------------------------

        // Composite Index: Fetch Master Product images sorted by DisplayOrder
        builder.HasIndex(i => new { i.ProductId, i.DisplayOrder });

        // Composite Index: Fetch Specific Variant images sorted by DisplayOrder
        builder.HasIndex(i => new { i.ProductVariantId, i.DisplayOrder });

        // -------------------------------------------------------------
        // RELATIONSHIPS CONFIGURATION
        // -------------------------------------------------------------

        // Master Product Relationship (Mandatory: Cascade Delete)
        builder.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional Variant Relationship (Cascade Delete when variant is removed)
        builder.HasOne(i => i.ProductVariant)
            .WithMany(pv => pv.Images)
            .HasForeignKey(i => i.ProductVariantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
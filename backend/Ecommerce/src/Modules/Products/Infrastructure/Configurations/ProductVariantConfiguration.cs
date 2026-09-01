using Ecommerce.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Products.Infrastructure.Persistence.Configurations;

// Fluent API configuration for ProductVariant entity (maps rules to EF Core).
public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    // Fix: Use EntityTypeBuilder<ProductVariant> instead of ProductVariantConfigurationBuilder
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    
    {
        // Enforces explicit table name
        builder.ToTable("products_variants");

        // Primary Key
        builder.HasKey(pv => pv.Id);

        // SKU constraints 
        builder.Property(pv => pv.SKU)
            .HasMaxLength(50)
            .IsRequired();

        // Precision mapping for ARS pricing (SQLite friendly, 18 total digits, 2 decimals)
        builder.Property(pv => pv.PriceArs)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(pv => pv.ComparisonPriceArs)
            .HasPrecision(18, 2);

        // Attributes string length restrictions
        builder.Property(pv => pv.Size)
            .HasMaxLength(20);

        builder.Property(pv => pv.Color)
            .HasMaxLength(30);

        builder.Property(pv => pv.HexColor)
            .HasMaxLength(10); // Format: "#RRGGBB"

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION
        // -------------------------------------------------------------

        // builder.HasIndex(pv => pv.SKU)
        //    .IsUnique();

        // Foreign Key Index: Essential for retrieving all variants under a master Product
        builder.HasIndex(pv => pv.ProductId);

        // Price Index: Accelerates filtering and sorting by price on catalog pages
        builder.HasIndex(pv => pv.PriceArs);

        // -------------------------------------------------------------
        // RELATIONSHIP CONFIGURATION
        // -------------------------------------------------------------

        // Master Product Relationship (Mandatory: Cascade Delete)
        // If a Master Product is deleted, all of its variants MUST be deleted.
        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
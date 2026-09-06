namespace Ecommerce.Products.Infrastructure.Configurations;

using Ecommerce.Products.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// Entity Framework Core Fluent API configuration for mapping the <see cref="ProductVariant"/> domain entity 
/// to the underlying database schema.
/// </summary>
public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    /// <summary>
    /// Configures database table mappings, column types, decimal precision, indexes, and relationships for the <see cref="ProductVariant"/> entity.
    /// </summary>
    /// <param name="builder">The builder providing the API to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        // 1. Table & Primary Key Mapping
        builder.ToTable("products_variants");

        // Primary key configuration (automatically creates a clustered index by default)
        builder.HasKey(pv => pv.Id);

        // 2. Property Constraints & Precision Settings
        builder.Property(pv => pv.Name)
            .HasMaxLength(220)
            .IsRequired();

        builder.Property(pv => pv.NormalizedName)
            .HasMaxLength(240)
            .IsRequired();
        
        builder.Property(pv => pv.MainImageUrl)
            .HasMaxLength(220);

        // Precision mapping for ARS currency (13 total digits, 2 decimal places)
        builder.Property(pv => pv.PriceArs)
            .HasPrecision(13, 2)
            .IsRequired();

        builder.Property(pv => pv.UnitCostArs)
            .HasPrecision(13, 2)
            .IsRequired();

        builder.Property(pv => pv.ComparisonPriceArs)
            .HasPrecision(13, 2);

        // Optional variant physical attributes formatting
        builder.Property(pv => pv.SKU)
            .HasMaxLength(50);

        builder.Property(pv => pv.Size)
            .HasMaxLength(20);

        builder.Property(v => v.Color)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired(false);

        builder.Property(pv => pv.DisplayColorName)
            .HasMaxLength(50);

        builder.Property(pv => pv.HexColor)
            .HasMaxLength(10); // Format: "#RRGGBB"

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION
        // -------------------------------------------------------------

        // Dynamically retrieve the mapped database column name for IsDeleted
        // to guarantee compile-time type safety across database conventions (e.g., snake_case).
        // var isDeletedColumn = builder.Property(pv => pv.IsDeleted)
        //     .Metadata.GetColumnName()!;

        // Filtered unique index allowing SKU reuse after soft deletion (commented out until SKU requirements are active)
        // builder.HasIndex(pv => pv.SKU)
        //     .HasFilter($"{isDeletedColumn} = 0")
        //     .IsUnique();

        // Foreign Key Index: Accelerates JOINs and variant searches by Master Product
        builder.HasIndex(pv => pv.ProductId);

        // Price Index: Accelerates price range filters and sorting (Min/Max)
        builder.HasIndex(pv => pv.PriceArs);

        // Search Index: Accelerates free text/natural language searches
        builder.HasIndex(pv => pv.NormalizedName);

        // -------------------------------------------------------------
        // RELATIONSHIP CONFIGURATION
        // -------------------------------------------------------------

        // Master Product Relationship (Mandatory foreign key with cascade behavior)
        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

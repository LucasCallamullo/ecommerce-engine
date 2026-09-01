using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Products.Domain.Entities;

namespace Ecommerce.Products.Infrastructure.Persistence.Configurations;

// Fluent API configuration for Brand entity (maps rules to EF Core).
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("products_brands");

        // PK - Automatically creates a Clustered Index (does not require HasIndex)
        builder.HasKey(b => b.Id);

        // Name constraints & Unique Index (prevents creating duplicated brand names)
        builder.Property(b => b.Name)
            .HasMaxLength(50)
            .IsRequired();

        // Slug constraints & Unique Index for URL routing
        builder.Property(b => b.Slug)
            .HasMaxLength(69)
            .IsRequired();

        // Optional fields string length limits
        builder.Property(b => b.Description)
            .HasMaxLength(100);

        builder.Property(b => b.ImageUrl)
            .HasMaxLength(200);

        // -------------------------------------------------------------
        // INDEXES FOR QUERY OPTIMIZATION
        // -------------------------------------------------------------

        builder.HasIndex(b => b.Slug)
            .IsUnique();

        // Single index for fetching active brands for UI listings/filters
        builder.HasIndex(b => b.IsActive);
    }
}
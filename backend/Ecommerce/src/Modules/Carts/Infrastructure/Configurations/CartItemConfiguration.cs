using Ecommerce.Carts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Carts.Infrastructure.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("carts_cart_item");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.VariantId)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .IsRequired();

        // Prevents duplicate rows for the same variant inside the same cart
        builder.HasIndex(i => new { i.CartId, i.VariantId })
            .IsUnique();
    }
}
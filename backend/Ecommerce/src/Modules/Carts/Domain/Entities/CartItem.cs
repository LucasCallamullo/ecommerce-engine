using Ecommerce.Shared.Common;

namespace Ecommerce.Carts.Domain.Entities;

public class CartItem : BaseEntity<int>
{
    public int CartId { get; set; }
    public int VariantId { get; set; }
    public int Quantity { get; set; }

    public Cart Cart { get; set; } = null!;
}
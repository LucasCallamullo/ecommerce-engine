namespace Ecommerce.Carts.Domain.Entities;

using Ecommerce.Shared.Common; 

public class Cart : BaseEntity<int>
{
    public Guid UserId { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
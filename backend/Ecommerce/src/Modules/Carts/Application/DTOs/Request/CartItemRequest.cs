namespace Ecommerce.Carts.Application.DTOs.Request;

public record AddCartItemRequest(
    int VariantId,
    int Quantity
);

public record UpdateCartItemRequest(
    int Quantity
);

public record MergeCartRequest(
    List<AddCartItemRequest> Items
);
using Ecommerce.Carts.Application.DTOs.Request;
using Ecommerce.Carts.Application.DTOs.Response;

namespace Ecommerce.Carts.Application.Interfaces;

public interface ICartService
{
    Task<CartResponse> GetMyCartAsync(Guid userId, CancellationToken ct = default);
    Task<CartResponse> AddItemAsync(Guid userId, AddCartItemRequest request, CancellationToken ct = default);
    Task<CartResponse> UpdateItemQuantityAsync(Guid userId, int variantId, UpdateCartItemRequest request, CancellationToken ct = default);
    Task<CartResponse> RemoveItemAsync(Guid userId, int variantId, CancellationToken ct = default);
    Task<CartResponse> MergeCartAsync(Guid userId, MergeCartRequest request, CancellationToken ct = default);
    Task ClearCartAsync(Guid userId, CancellationToken ct = default);
}
using Ecommerce.Carts.Application.DTOs.Request;
using Ecommerce.Carts.Application.DTOs.Response;
using Ecommerce.Carts.Application.Interfaces;
using Ecommerce.Carts.Domain.Entities;
using Ecommerce.Products.Contracts.Interfaces;
using Ecommerce.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Carts.Application.Services.Internals;

public class CartService : ICartService
{
    private readonly AppDbContext _context;
    private readonly IProductContract _productContract;

    public CartService(AppDbContext context, IProductContract productContract)
    {
        _context = context;
        _productContract = productContract;
    }

    public async Task<CartResponse> GetMyCartAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await GetOrCreateCartEntityAsync(userId, ct);
        return await BuildCartResponseAsync(cart, ct);
    }

    public async Task<CartResponse> AddItemAsync(Guid userId, AddCartItemRequest request, CancellationToken ct = default)
    {
        var variant = await _productContract.GetVariantByIdAsync(request.VariantId, ct)
            ?? throw new KeyNotFoundException($"Product variant with ID {request.VariantId} was not found.");

        if (!variant.IsActive)
            throw new InvalidOperationException("This product variant is currently unavailable.");

        var cart = await GetOrCreateCartEntityAsync(userId, ct);
        var existingItem = cart.Items.FirstOrDefault(i => i.VariantId == request.VariantId);

        var targetQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;

        if (targetQuantity > variant.Stock)
            throw new InvalidOperationException($"Insufficient stock. Only {variant.Stock} unit(s) available.");

        if (existingItem is null)
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                VariantId = request.VariantId,
                Quantity = request.Quantity
            });
        }
        else
        {
            existingItem.Quantity = targetQuantity;
        }

        await _context.SaveChangesAsync(ct);
        return await BuildCartResponseAsync(cart, ct);
    }

    public async Task<CartResponse> UpdateItemQuantityAsync(
        Guid userId,
        int variantId,
        UpdateCartItemRequest request,
        CancellationToken ct = default)
    {
        var cart = await GetOrCreateCartEntityAsync(userId, ct);
        var item = cart.Items.FirstOrDefault(i => i.VariantId == variantId)
            ?? throw new KeyNotFoundException($"Variant {variantId} is not in your cart.");

        var variant = await _productContract.GetVariantByIdAsync(variantId, ct)
            ?? throw new KeyNotFoundException($"Product variant with ID {variantId} no longer exists.");

        if (!variant.IsActive)
            throw new InvalidOperationException("This product variant is no longer available.");

        if (request.Quantity > variant.Stock)
            throw new InvalidOperationException($"Insufficient stock. Only {variant.Stock} unit(s) available.");

        item.Quantity = request.Quantity;
        await _context.SaveChangesAsync(ct);

        return await BuildCartResponseAsync(cart, ct);
    }

    public async Task<CartResponse> RemoveItemAsync(Guid userId, int variantId, CancellationToken ct = default)
    {
        var cart = await GetOrCreateCartEntityAsync(userId, ct);
        var item = cart.Items.FirstOrDefault(i => i.VariantId == variantId);

        if (item is not null)
        {
            // Physical delete bypassing AppDbContext's soft-delete interceptor
            await _context.Set<CartItem>()
                .Where(i => i.Id == item.Id)
                .ExecuteDeleteAsync(ct);

            cart.Items.Remove(item);
        }

        return await BuildCartResponseAsync(cart, ct);
    }

    public async Task<CartResponse> MergeCartAsync(Guid userId, MergeCartRequest request, CancellationToken ct = default)
    {
        var cart = await GetOrCreateCartEntityAsync(userId, ct);

        if (request.Items.Count == 0)
            return await BuildCartResponseAsync(cart, ct);

        var groupedGuestItems = request.Items
            .GroupBy(i => i.VariantId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        var variantsMap = await _productContract.GetVariantsByIdsAsync(groupedGuestItems.Keys, ct);

        foreach (var (variantId, guestQty) in groupedGuestItems)
        {
            if (!variantsMap.TryGetValue(variantId, out var variant) || !variant.IsActive || variant.Stock <= 0)
                continue;

            var existingItem = cart.Items.FirstOrDefault(i => i.VariantId == variantId);
            var combinedQty = (existingItem?.Quantity ?? 0) + guestQty;
            var finalQty = Math.Min(combinedQty, variant.Stock);

            if (existingItem is null)
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cart.Id,
                    VariantId = variantId,
                    Quantity = finalQty
                });
            }
            else
            {
                existingItem.Quantity = finalQty;
            }
        }

        await _context.SaveChangesAsync(ct);
        return await BuildCartResponseAsync(cart, ct);
    }

    public async Task ClearCartAsync(Guid userId, CancellationToken ct = default)
    {
        var cartId = await _context.Set<Cart>()
            .Where(c => c.UserId == userId)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(ct);

        if (cartId is null) return;

        await _context.Set<CartItem>()
            .Where(i => i.CartId == cartId.Value)
            .ExecuteDeleteAsync(ct);
    }

    private async Task<Cart> GetOrCreateCartEntityAsync(Guid userId, CancellationToken ct)
    {
        var cart = await _context.Set<Cart>()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (cart is not null) return cart;

        cart = new Cart { UserId = userId };
        _context.Set<Cart>().Add(cart);
        await _context.SaveChangesAsync(ct);

        return cart;
    }

    private async Task<CartResponse> BuildCartResponseAsync(Cart cart, CancellationToken ct)
    {
        if (cart.Items.Count == 0)
            return new CartResponse(cart.Id, cart.UserId, [], 0, 0m, 0m, 0m, false);

        var variantIds = cart.Items.Select(i => i.VariantId);
        var variantsMap = await _productContract.GetVariantsByIdsAsync(variantIds, ct);

        var itemResponses = new List<CartItemResponse>();
        decimal totalOriginal = 0m;
        decimal totalFinal = 0m;
        int totalItems = 0;
        bool hasStockIssues = false;

        foreach (var item in cart.Items)
        {
            if (!variantsMap.TryGetValue(item.VariantId, out var variant))
            {
                hasStockIssues = true;
                itemResponses.Add(new CartItemResponse(
                    item.VariantId,
                    string.Empty,
                    "Unavailable Product",
                    null,
                    null,
                    null,
                    item.Quantity,
                    0,
                    0m,
                    null,
                    0,
                    0m,
                    0m,
                    false,
                    "This product is no longer available."
                ));
                continue;
            }

            bool isAvailable = variant.IsActive && variant.Stock >= item.Quantity;
            string? warning = null;

            if (!variant.IsActive || variant.Stock == 0)
            {
                hasStockIssues = true;
                warning = "Out of stock.";
            }
            else if (variant.Stock < item.Quantity)
            {
                hasStockIssues = true;
                warning = $"Only {variant.Stock} unit(s) left in stock.";
            }

            var lineOriginal = variant.PriceArs * item.Quantity;
            var lineFinal = variant.FinalPriceArs * item.Quantity;

            totalOriginal += lineOriginal;
            totalFinal += lineFinal;
            totalItems += item.Quantity;

            itemResponses.Add(new CartItemResponse(
                variant.Id,
                variant.ProductSlug,
                variant.VariantName,
                variant.MainImageUrl,
                variant.Size,
                variant.Color,
                item.Quantity,
                variant.Stock,
                variant.PriceArs,
                variant.ComparisonPriceArs,
                variant.DiscountPercentageArs,
                variant.FinalPriceArs,
                lineFinal,
                isAvailable,
                warning
            ));
        }

        return new CartResponse(
            cart.Id,
            cart.UserId,
            itemResponses,
            totalItems,
            totalOriginal,
            totalOriginal - totalFinal,
            totalFinal,
            hasStockIssues
        );
    }
}
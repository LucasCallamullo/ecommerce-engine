using System.Net;
using Ecommerce.Carts.Application.DTOs.Request;
using Ecommerce.Carts.Application.DTOs.Response;
using Ecommerce.Carts.Application.Interfaces;
using Ecommerce.Shared.API;
using Ecommerce.Shared.Auth.Interfaces;
using Ecommerce.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Carts.API.Controllers;

/// <summary>
/// REST API Controller managing the authenticated user's shopping cart.
/// </summary>
[Route("api/v1/cart")]
public class CartController(
    ICartService cartService,
    ICurrentUserProvider currentUser) : ApiControllerBase
{
    private readonly ICartService _cartService = cartService;
    private readonly ICurrentUserProvider _currentUser = currentUser;

    /// <summary>Retrieves the authenticated user's active shopping cart.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyCart(CancellationToken ct = default)
    {
        var cart = await _cartService.GetMyCartAsync(GetUserId(), ct);
        return Ok(cart);
    }

    /// <summary>Adds a product variant to the authenticated user's cart.</summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request, CancellationToken ct = default)
    {
        var cart = await _cartService.AddItemAsync(GetUserId(), request, ct);
        return Ok(cart);
    }

    /// <summary>Updates the quantity of an existing variant in the user's cart.</summary>
    [HttpPatch("items/{variantId:int}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItemQuantity(
        [FromRoute] int variantId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken ct = default)
    {
        var cart = await _cartService.UpdateItemQuantityAsync(GetUserId(), variantId, request, ct);
        return Ok(cart);
    }

    /// <summary>Removes a specific product variant from the user's cart.</summary>
    [HttpDelete("items/{variantId:int}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveItem([FromRoute] int variantId, CancellationToken ct = default)
    {
        var cart = await _cartService.RemoveItemAsync(GetUserId(), variantId, ct);
        return Ok(cart);
    }

    /// <summary>Merges guest localStorage cart items into the user's persistent cart upon login.</summary>
    [HttpPost("merge")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MergeCart([FromBody] MergeCartRequest request, CancellationToken ct = default)
    {
        var cart = await _cartService.MergeCartAsync(GetUserId(), request, ct);
        return Ok(cart);
    }

    /// <summary>Clears all items from the authenticated user's cart.</summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ClearCart(CancellationToken ct = default)
    {
        await _cartService.ClearCartAsync(GetUserId(), ct);
        return NoContent();
    }

    private Guid GetUserId() =>
        _currentUser.UserId ?? 
            throw new AppException("User identifier claim is missing from token.", HttpStatusCode.Unauthorized);
}
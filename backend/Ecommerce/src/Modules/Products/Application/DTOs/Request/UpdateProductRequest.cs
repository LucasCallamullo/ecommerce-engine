namespace Ecommerce.Products.Application.DTOs.Request;

/// <summary>
/// Payload required to update an existing product.
/// </summary>
public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    int CategoryId
);
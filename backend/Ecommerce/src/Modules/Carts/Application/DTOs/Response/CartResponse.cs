namespace Ecommerce.Carts.Application.DTOs.Response;

public record CartItemResponse(
    int VariantId,
    string ProductSlug,
    string VariantName,
    string? MainImageUrl,
    string? Size,
    string? Color,
    int Quantity,
    int AvailableStock,
    decimal UnitPriceArs,
    decimal? ComparisonPriceArs,
    int DiscountPercentageArs,
    decimal FinalUnitPriceArs,
    decimal SubtotalArs,
    bool IsAvailable,
    string? StockWarning
);

public record CartResponse(
    int CartId,
    Guid UserId,
    List<CartItemResponse> Items,
    int TotalItems,
    decimal TotalOriginalArs,
    decimal TotalDiscountArs,
    decimal TotalFinalArs,
    bool HasStockIssues
);
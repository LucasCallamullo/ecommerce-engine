namespace Ecommerce.Products.Contracts.DTOs;

public record ProductVariantContractDto(
    int Id,                        // 1
    string ProductSlug,            // 2
    string VariantName,            // 3
    string? MainImageUrl,          // 4
    string? Size,                  // 5
    string? Color,                 // 6
    decimal PriceArs,              // 7
    decimal? ComparisonPriceArs,   // 8
    int DiscountPercentageArs,     // 9
    decimal FinalPriceArs,         // 10
    int Stock,                     // 11
    bool IsActive                  // 12
);
// Ecommerce.Product.Application/Services/VariantService.cs
using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Domain.Entities;
using Ecommerce.Shared.Database;

namespace Ecommerce.Products.Application.Services;

public class VariantService(AppDbContext context) : IVariantService
{
    private readonly AppDbContext _context = context;

    // Maps a collection of variant request DTOs into domain entities attached to a parent Product.
    public List<ProductVariant> CreateVariantsFromRequests(
        List<ProductCreateVariantRequest>? variantRequests, 
        Product product)
    {
        // Step 1: Validate input collection; return an empty list if null or empty.
        if (variantRequests is null || variantRequests.Count == 0) 
            return [];

        // Step 2: Project each request DTO to a ProductVariant entity.
        return variantRequests.Select(v => new ProductVariant
        {
            SKU = v.SKU ?? GenerateSku(),
            PriceArs = v.PriceArs,
            ComparisonPriceArs = v.ComparisonPriceArs,
            DiscountArs = v.DiscountArs,
            Stock = v.Stock,
            Size = v.Size,
            Color = v.Color,
            HexColor = v.HexColor,
            // Step 4: Establish the bi-directional navigation reference with the parent Product.
            Product = product 
        }).ToList();
    }

    // Helper method to generate a fallback unique SKU identifier.
    public string GenerateSku()
    {
        // Step 1: Generate a pseudo-random 4-digit numerical suffix using thread-safe Random.Shared.
        var random = Random.Shared.Next(1000, 9999).ToString();
        var random2 = Random.Shared.Next(1000, 9999).ToString();
        return $"SKU-{random}-{random2}";
    }
}
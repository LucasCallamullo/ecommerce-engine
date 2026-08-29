using Ecommerce.Shared.Common;

namespace Ecommerce.Products.Domain.Entities;

/// Represents a physical sellable inventory item (SKU) under a master Product.
/// Encapsulates transactional attributes such as pricing in ARS, stock levels, sizes, and colors.
/// Note: At least one default ProductVariant is automatically created per master Product.
/// 
// - Id (int, Primary Key)
// - CreatedAt (DateTime, UTC timestamp upon insertion)
// - UpdatedAt (DateTime?, nullable UTC timestamp upon modification)
// - IsDeleted (bool, soft delete logical flag)
public class ProductVariant : BaseEntity<int>
{
    /// Unique stock-keeping unit code (e.g., "NK-WND-BLK-M").
    public string SKU { get; set; } = string.Empty;

    /// Current selling price in ARS.
    public decimal PriceArs { get; set; }

    /// Optional reference or original list price in ARS for strike-through discount UI display.
    public decimal? ComparisonPriceArs { get; set; }

    /// Fixed discount amount or percentage applied to this variant in ARS.
    public int DiscountArs { get; set; }

    /// Total available stock quantity in inventory.
    public int Stock { get; set; }

    /// Variation size attribute (e.g., "S", "M", "L", "42").
    public string? Size { get; set; }

    /// Variation color name attribute (e.g., "Red", "Black").
    public string? Color { get; set; }

    /// Hexadecimal color code for visual swatch selectors on frontend UI (e.g., "#FF0000").
    public string? HexColor { get; set; }

    /// Foreign Key referencing the parent master Product. DONT ALLOW NULL
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// Collection of images specific to this variant (overrides or complements master Product images).
    public ICollection<ProductImage> Images { get; set; } = [];

    /* ==================================================================
        Methods for business rules 
    ================================================================== */

    /// <summary>
    /// Adjusts the current stock quantity by adding or subtracting the specified value.
    /// Ensures that inventory levels never drop below zero.
    /// </summary>
    /// <param name="quantity">The amount to adjust stock by (positive to restock, negative to reduce).</param>
    /// <exception cref="InvalidOperationException">Thrown when the resulting stock level would be negative.</exception>
    public void UpdateStock(int quantity)
    {
        if (Stock + quantity < 0)
            throw new InvalidOperationException("Stock cannot be negative.");

        Stock += quantity;
    }

    /// <summary>
    /// Updates the current selling price in ARS.
    /// Validates that the new price is greater than zero before applying the update.
    /// </summary>
    /// <param name="newPrice">The new selling price in ARS.</param>
    /// <exception cref="ArgumentException">Thrown when the provided price is less than or equal to zero.</exception>
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero.", nameof(newPrice));

        this.PriceArs = newPrice;
    }
}
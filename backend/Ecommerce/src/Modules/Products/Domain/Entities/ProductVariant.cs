using Ecommerce.Shared.Common;

namespace Ecommerce.Products.Domain.Entities;

/// <summary>Represents a physical sellable inventory item (SKU) under a master <see cref="Product"/>.</summary>
/// <remarks>
/// Encapsulates transactional attributes such as pricing in ARS, stock levels, sizes, and colors.
/// Note: At least one default <see cref="ProductVariant"/> is automatically created per master product.
/// 
/// Inherits from <see cref="BaseEntity{TKey}"/> to provide core auditing and soft-delete attributes:
/// <list type="bullet">
/// <item><description><c>Id</c>: Integer primary key identifier.</description></item>
/// <item><description><c>CreatedAt</c>: UTC timestamp recorded upon insertion.</description></item>
/// <item><description><c>UpdatedAt</c>: Nullable UTC timestamp recorded upon modification.</description></item>
/// <item><description><c>IsDeleted</c>: Boolean flag for logical soft deletion.</description></item>
/// </list>
/// </remarks>
public class ProductVariant : BaseEntity<int>
{
    /// <summary>
    /// Gets or sets the unique stock-keeping unit code (e.g., "NK-WND-BLK-M").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique stock-keeping unit code (e.g., "NK-WND-BLK-M").
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>Gets or sets the primary image URL or path for catalog listings.</summary>
    /// <remarks>
    /// Denormalized field used to prevent performance bottlenecks and frequent JOIN operations against <see cref="ProductImage"/>.
    /// </remarks>
    public string? MainImageUrl { get; set; }

    //? =====================================================
    //?        Pricing PROPERTIES
    //? =====================================================

    /// <summary>
    /// Gets or sets the current selling price in ARS.
    /// </summary>
    public decimal PriceArs { get; set; }

    /// <summary>
    /// Gets or sets the internal unit cost in ARS paid to suppliers (confidential, used for profit analytics).
    /// </summary>
    public decimal? UnitCostArs { get; set; }

    /// <summary>
    /// Gets or sets the optional original or reference price in ARS for strike-through UI display.
    /// If provided, this value must be greater than PriceArs.
    /// </summary>
    public decimal? ComparisonPriceArs { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage (0 to 100) applied to this variant.
    /// </summary>
    public int DiscountPercentageArs { get; set; }

    /// <summary>
    /// Gets or sets the total available physical stock quantity in inventory.
    /// </summary>
    public int Stock { get; set; }

    //? =====================================================
    //?        OTHER PROPERTIES
    //? =====================================================

    /// <summary>
    /// Gets or sets the unique stock-keeping unit code (e.g., "NK-WND-BLK-M").
    /// </summary>
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique stock-keeping unit code (e.g., "NK-WND-BLK-M").
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the variation size attribute (e.g., "S", "M", "L", "42").
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// Gets or sets the variation color name attribute (e.g., "Red", "Black").
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Optional override for gender agreement or specific names (e.g., "White", "Navy Blue")
    /// </summary>
    public string? DisplayColorName { get; set; }

    /// <summary>
    /// Gets or sets the hexadecimal color code for visual swatch selectors on the frontend UI (e.g., "#FF0000").
    /// </summary>
    public string? HexColor { get; set; }

    //? ===========================================================================
    //?              FK Relations
    //? ===========================================================================

    /// <summary>Gets or sets the foreign key referencing the parent master <see cref="Product"/>.</summary>
    public int ProductId { get; set; }

    /// <summary>Gets or sets the navigation property for the parent master <see cref="Product"/>.</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Gets or sets the collection of images specific to this variant, overriding or complementing master product images.</summary>
    public ICollection<ProductImage> Images { get; set; } = [];

    //? ===========================================================================
    //?             Methods for business rules
    //? ===========================================================================

    /// <summary>Adjusts the current stock quantity by adding or subtracting the specified value.</summary>
    /// <param name="quantity">The amount to adjust stock by (positive to restock, negative to reduce).</param>
    /// <exception cref="InvalidOperationException">Thrown when the resulting stock level would drop below zero.</exception>
    public void UpdateStock(int quantity)
    {
        if (Stock + quantity < 0)
            throw new InvalidOperationException("Stock cannot be negative.");

        Stock += quantity;
    }

    /// <summary>Updates the current selling price in ARS after validating that it is greater than zero.</summary>
    /// <param name="newPrice">The new selling price in ARS.</param>
    /// <exception cref="ArgumentException">Thrown when the provided price is less than or equal to zero.</exception>
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero.", nameof(newPrice));

        this.PriceArs = newPrice;
    }
}
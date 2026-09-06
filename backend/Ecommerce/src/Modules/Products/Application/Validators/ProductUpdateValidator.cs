using Ecommerce.Products.Application.DTOs.Request;
using FluentValidation;

namespace Ecommerce.Products.Application.Validators;

/// <summary> Validator responsible for enforcing conditional business rules on master product update requests.</summary>
public class ProductUpdateValidator : AbstractValidator<ProductUpdateRequest>
{
    /// <summary> Initializes validation rules for <see cref="ProductUpdateRequest"/>.</summary>
    public ProductUpdateValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(150).WithMessage("Product name cannot exceed 150 characters.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        // Stupids checks for entities
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be greater than zero.")
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.SubcategoryId)
            .GreaterThan(0).WithMessage("SubcategoryId must be greater than zero.")
            .Must((request, subcategoryId) => request.CategoryId.HasValue)
                .WithMessage("CategoryId is required when a SubcategoryId is provided.")
            .When(x => x.SubcategoryId.HasValue);

        RuleFor(x => x.BrandId)
            .GreaterThan(0).WithMessage("BrandId must be greater than zero.")
            .When(x => x.BrandId.HasValue);

        // If the 'Variant' nested object is null (omitted in the payload), skip validation for it.
        // Otherwise, delegate validation of the nested properties to ProductVariantUpdateValidator.
        // Note: The '!' operator avoids nullability compiler warnings since '.When()' ensures it won't be null at runtime.
        // RuleFor(x => x.Variant!)
        //    .SetValidator(new ProductVariantUpdateValidator())
        //    .When(x => x.Variant != null);
    }
}

/// <summary> Validator responsible for enforcing validation rules on variant update payloads.</summary>
public class ProductVariantUpdateValidator : AbstractValidator<ProductVariantUpdateRequest>
{
    /// <summary> Initializes validation rules for <see cref="ProductVariantUpdateRequest"/>.</summary>
    public ProductVariantUpdateValidator()
    {
        RuleFor(x => x.PriceArs)
            .GreaterThan(0).WithMessage("Price $ARS must be greater than zero.")
            .When(x => x.PriceArs.HasValue);

        RuleFor(x => x.UnitCostArs)
            .GreaterThan(0).WithMessage("Unit cost $ARS must be greater than zero.")
            .When(x => x.UnitCostArs.HasValue);

        RuleFor(x => x.ComparisonPriceArs)
            .GreaterThanOrEqualTo(0).WithMessage("Comparison Price $ARS must be greater or equal than zero.")
            .When(x => x.ComparisonPriceArs.HasValue);

        RuleFor(x => x.DiscountPercentageArs)
            .InclusiveBetween(0, 100)
            .WithMessage("Discount percentage must be between 0 and 100.")
            .When(x => x.DiscountPercentageArs.HasValue);
            
        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.")
            .When(x => x.Stock.HasValue);

        // 2. Physical Attribute Validations (Synchronized with EF Core MaxLength)
        RuleFor(x => x.SKU)
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

        RuleFor(x => x.Size)
            .MaximumLength(20).WithMessage("Size must not exceed 20 characters.");

        RuleFor(x => x.BaseColor)
            .MaximumLength(30).WithMessage("Color must not exceed 30 characters.");

        RuleFor(x => x.DisplayColorName)
            .MaximumLength(50).WithMessage("Color display name override must not exceed 50 characters.");

        RuleFor(x => x.HexColor)
            .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$")
            .When(x => !string.IsNullOrEmpty(x.HexColor))
            .WithMessage("HexColor must be a valid hex color format (e.g., #FF0000).");
    }
}
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

        RuleFor(x => x.ComparisonPriceArs)
            .GreaterThan(0).WithMessage("Comparison Price $ARS must be greater than zero.")
            .When(x => x.ComparisonPriceArs.HasValue);

        RuleFor(x => x.DiscountArs)
            .GreaterThanOrEqualTo(0).WithMessage("Discount percentual $ARS must be greater than zero.")
            .When(x => x.DiscountArs.HasValue);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.")
            .When(x => x.Stock.HasValue);

        RuleFor(x => x.HexColor)
            .Matches("^#(?:[0-9a-fA-F]{3}){1,2}$")
            .WithMessage("HexColor must be a valid hex color format (e.g., #FF0000).")
            .When(x => !string.IsNullOrEmpty(x.HexColor));
    }
}
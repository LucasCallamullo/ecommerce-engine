using Ecommerce.Products.Application.DTOs.Request;
using FluentValidation;

namespace Ecommerce.Products.Application.Validators;

/// <summary> Validator responsible for enforcing domain and structural validation rules on master product creation requests. </summary>
public class ProductCreateValidator : AbstractValidator<ProductCreateRequest>
{
    /// <summary> Initializes validation rules for <see cref="ProductCreateRequest"/>.</summary>
    public ProductCreateValidator()
    {
        // 1. Master Product Attributes Validation
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(150).WithMessage("Product name cannot exceed 150 characters.");

        // 2. Initial Variants Collection Validation (Ensures at least one variant is provided)
        RuleFor(x => x.Variants)
            .NotNull().WithMessage("Variants list cannot be null.")
            .Must(variants => variants.Count > 0)
            .WithMessage("At least one product variant must be provided when creating a product.");

        // 3. Nested Validation: Applies ProductCreateVariantValidator rules to each element in the collection
        RuleForEach(x => x.Variants)
            .SetValidator(new ProductCreateVariantValidator());

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
    }
}

/// <summary> Validator responsible for enforcing business rules on individual product variant creation payloads.</summary>
public class ProductCreateVariantValidator : AbstractValidator<ProductCreateVariantRequest>
{
    /// <summary> Initializes validation rules for <see cref="ProductCreateVariantRequest"/>.</summary>
    public ProductCreateVariantValidator()
    {
        // 1. Numerical and Financial Validations
        RuleFor(x => x.PriceArs)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.UnitCostArs)
            .GreaterThan(0).WithMessage("Unit cost must be greater than zero.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(x => x.ComparisonPriceArs)
            .GreaterThanOrEqualTo(0).WithMessage("Comparison Price $ARS must be greater than zero.")
            .When(x => x.ComparisonPriceArs.HasValue);

        RuleFor(x => x.DiscountPercentageArs)
            .InclusiveBetween(0, 100)
            .WithMessage("Discount percentage must be between 0 and 100.");

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
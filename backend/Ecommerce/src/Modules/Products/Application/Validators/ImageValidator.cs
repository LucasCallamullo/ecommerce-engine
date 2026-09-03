namespace Ecommerce.Products.Application.Validators;

using Ecommerce.Products.Application.Request.DTOs;
using Ecommerce.Shared.Common.Validations;
using FluentValidation;

// + ===========================================================================
// +         Product Image FluentValidation Rules
// + ===========================================================================

/// <summary>
/// Validator enforcing database and business rules for <see cref="ProductImageCreateRequest"/>.
/// </summary>
public class ProductImageCreateRequestValidator : AbstractValidator<ProductImageCreateRequest>
{
    public ProductImageCreateRequestValidator()
    {
        // Url constraint: Required, MaxLength(220) in DB, valid absolute HTTP/HTTPS URL format
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Image URL is required.")
            .MaximumLength(220).WithMessage("Image URL must not exceed 220 characters.")
            .Must(ValidationHelpers.BeAValidUrl).WithMessage("Image URL format is invalid.")
            .When(x => !string.IsNullOrEmpty(x.Url));

        // AltText constraint: Optional, MaxLength(200) in DB
        RuleFor(x => x.AltText)
            .MaximumLength(200).WithMessage("Alternative text must not exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.AltText));

        // DisplayOrder constraint: Must be 0 or greater
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative integer.")
            .When(x => x.DisplayOrder.HasValue);

        // Foreign Key constraints: Foreign keys must be valid positive integers
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID must be a valid positive integer.");

        RuleFor(x => x.ProductVariantId)
            .GreaterThan(0).WithMessage("Product Variant ID must be a valid positive integer.")
            .When(x => x.ProductVariantId.HasValue);
    }
}

/// <summary>
/// Validator enforcing database and business rules for <see cref="ProductImageUpdateRequest"/>.
/// </summary>
public class ProductImageUpdateRequestValidator : AbstractValidator<ProductImageUpdateRequest>
{
    public ProductImageUpdateRequestValidator()
    {
        // Url constraint: Required, MaxLength(220) in DB, valid absolute HTTP/HTTPS URL format
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Image URL is required.")
            .MaximumLength(220).WithMessage("Image URL must not exceed 220 characters.")
            .Must(ValidationHelpers.BeAValidUrl).WithMessage("Image URL format is invalid.")
            .When(x => !string.IsNullOrEmpty(x.Url));

        // AltText constraint: Optional, MaxLength(200) in DB
        RuleFor(x => x.AltText)
            .MaximumLength(200).WithMessage("Alternative text must not exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.AltText));

        // DisplayOrder constraint: Must be 0 or greater
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative integer.")
            .When(x => x.DisplayOrder.HasValue);

        // Foreign Key constraint: Variant ID must be a valid positive integer if provided
        RuleFor(x => x.ProductVariantId)
            .GreaterThan(0).WithMessage("Product Variant ID must be a valid positive integer.")
            .When(x => x.ProductVariantId.HasValue);
    }
}
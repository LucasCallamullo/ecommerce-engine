namespace Ecommerce.Products.Application.Validators;

using FluentValidation;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Shared.Common.Validations;

/// <summary>
/// Validator enforcing business and database integrity rules for <see cref="CategoryCreateRequest"/>.
/// </summary>
public class CategoryCreateValidator : AbstractValidator<CategoryCreateRequest>
{
    public CategoryCreateValidator()
    {
        // Name constraint: MaxLength(50) in DB
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(50).WithMessage("Category name must not exceed 50 characters.");

        // Description constraint: MaxLength(100) in DB
        RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage("Description must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // ImageUrl constraint: MaxLength(200) in DB
        RuleFor(x => x.ImageUrl)
            .MaximumLength(200).WithMessage("Image URL must not exceed 200 characters.")
            .Must(ValidationHelpers.BeAValidUrl).WithMessage("Image URL format is invalid.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        // ParentCategoryId constraint: Foreign keys must be positive integers if provided
        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0).WithMessage("Parent category ID must be a valid positive integer.")
            .When(x => x.ParentCategoryId.HasValue);
    }
}


/// <summary>
/// Validator enforcing business and database integrity rules for <see cref="CategoryUpdateRequest"/>.
/// </summary>
public class CategoryUpdateValidator : AbstractValidator<CategoryUpdateRequest>
{
    public CategoryUpdateValidator()
    {
        // Name constraint: MaxLength(50) in DB
        RuleFor(x => x.Name)
            .MaximumLength(50).WithMessage("Category name must not exceed 50 characters.");

        // Description constraint: MaxLength(100) in DB
        RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage("Description must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // ImageUrl constraint: MaxLength(200) in DB
        RuleFor(x => x.ImageUrl)
            .MaximumLength(200).WithMessage("Image URL must not exceed 200 characters.")
            .Must(ValidationHelpers.BeAValidUrl).WithMessage("Image URL format is invalid.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        // ParentCategoryId constraint: Foreign keys must be positive integers if provided
        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0).WithMessage("Parent category ID must be a valid positive integer.")
            .When(x => x.ParentCategoryId.HasValue);
    }
}
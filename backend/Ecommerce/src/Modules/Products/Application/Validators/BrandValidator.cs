namespace Ecommerce.Products.Application.Validators;

using Ecommerce.Products.Application.DTOs.Request;
using FluentValidation;

/// <summary>
/// Validator enforcing validation rules for BrandCreateRequest DTO matching database constraints.
/// </summary>
public class BrandCreateValidator : AbstractValidator<BrandCreateRequest>
{
    public BrandCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required.")
            .MaximumLength(50).WithMessage("Brand name must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage("Description must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(200).WithMessage("Image URL must not exceed 200 characters.")
            .Must(BeAValidUrl).WithMessage("Image URL must be a valid absolute or relative path.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _);
    }
}


/// <summary>
/// Validator enforcing validation rules for BrandUpdateRequest DTO matching database constraints.
/// </summary>
public class BrandUpdateValidator : AbstractValidator<BrandUpdateRequest>
{
    public BrandUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required.")
            .MaximumLength(50).WithMessage("Brand name must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage("Description must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(200).WithMessage("Image URL must not exceed 200 characters.")
            .Must(BeAValidUrl).WithMessage("Image URL must be a valid absolute or relative path.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _);
    }
}
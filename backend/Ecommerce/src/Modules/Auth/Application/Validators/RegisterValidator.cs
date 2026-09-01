using Ecommerce.Auth.Application.DTOs.Request;
using FluentValidation;

namespace Ecommerce.Auth.Application.Validators.Request;

/// <summary>
/// Validator enforcing structural rules and validation constraints for <see cref="RegisterRequest"/>,
/// aligned with database configuration parameters.
/// </summary>
public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(120).WithMessage("Email address cannot exceed 120 characters.");

        // password rule 
        // ! maybe changes in the future
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(4).WithMessage("Password must be at least 4 characters long.")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        When(x => !string.IsNullOrEmpty(x.Cellphone), () =>
        {
            RuleFor(x => x.Cellphone)
                .MaximumLength(30).WithMessage("Cellphone number cannot exceed 30 characters.");
        });

        When(x => !string.IsNullOrEmpty(x.Dni), () =>
        {
            RuleFor(x => x.Dni)
                .MaximumLength(20).WithMessage("DNI cannot exceed 20 characters.");
        });
    }
}
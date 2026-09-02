using FluentValidation;
using Ecommerce.Auth.Application.DTOs.Request;

namespace Ecommerce.Auth.Application.Validators;

/// <summary>
/// Validator enforcing structural rules and constraints for <see cref="LoginRequest"/>.
/// </summary>
public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(120).WithMessage("Email cannot exceed 120 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(4).WithMessage("Password must be at least 6 characters long.");
    }
}
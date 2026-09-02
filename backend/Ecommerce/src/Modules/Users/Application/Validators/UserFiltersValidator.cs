namespace Ecommerce.Users.Application.Validators;

using FluentValidation;
using Ecommerce.Users.Application.DTOs.Request;

/// <summary>
/// Validator for user querying and pagination parameters (<see cref="UserFilterParams"/>).
/// Prevents invalid page bounds and restricts query string lengths.
/// </summary>
public class UserFilterParamsValidator : AbstractValidator<UserFilterParams>
{
    public UserFilterParamsValidator()
    {
        RuleFor(x => x.SearchTerm!)
            .MaximumLength(100).WithMessage("Search term must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.Email!)
            .MaximumLength(150).WithMessage("Email filter must not exceed 150 characters.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100 records per request.");
    }
}
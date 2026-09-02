namespace Ecommerce.Users.Application.Validators;

using FluentValidation;
using Ecommerce.Shared.Auth.Enums;
using Ecommerce.Users.Application.DTOs.Request;

/// <summary>
/// Validator responsible for enforcing domain and data integrity rules for user profile updates (<see cref="UpdateUserProfile"/>).
/// 
/// <para>
/// <b>Validation Strategy:</b><br/>
/// Evaluates input rules conditionally using <c>.When(...)</c> clauses to seamlessly support partial updates (PATCH operations).
/// If a property is omitted (<c>null</c>) in the request payload, its corresponding validation rules are skipped.
/// </para>
/// </summary>
public class UserProfileValidator : AbstractValidator<UpdateUserProfile>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfileValidator"/> class and defines validation rules.
    /// </summary>
    public UserProfileValidator()
    {
        RuleFor(x => x.FirstName!)
            .NotEmpty().WithMessage("First name cannot be empty when provided.")
            .MaximumLength(40).WithMessage("First name must not exceed 40 characters.")
            .When(x => x.FirstName != null);

        RuleFor(x => x.LastName!)
            .NotEmpty().WithMessage("Last name cannot be empty when provided.")
            .MaximumLength(40).WithMessage("Last name must not exceed 40 characters.")
            .When(x => x.LastName != null);

        RuleFor(x => x.Cellphone!)
            .MaximumLength(30).WithMessage("Cellphone number must not exceed 30 characters.")
            .Matches(@"^\+?[0-9\s\-()]+$").WithMessage("Cellphone contains invalid characters.")
            .When(x => x.Cellphone != null);

        RuleFor(x => x.Dni!)
            .MaximumLength(20).WithMessage("DNI must not exceed 20 characters.")
            .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("DNI contains invalid characters.")
            .When(x => x.Dni != null);
    }
}

/// <summary>
/// Validator for administrative role update requests (<see cref="UpdateUserRol"/>).
/// Ensures the target email is valid and all provided roles exist within <see cref="UserRoleEnum"/>.
/// </summary>
public class UpdateUserRolValidator : AbstractValidator<UpdateUserRol>
{
    public UpdateUserRolValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Target user email is required.")
            .EmailAddress().WithMessage("A valid email address must be provided.")
            .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");

        RuleFor(x => x.Roles)
            .NotNull().WithMessage("Roles collection cannot be null.")
            .NotEmpty().WithMessage("At least one role must be assigned to the user.");

        // RuleForEach inspects every individual string element inside the List<string> Roles
        RuleForEach(x => x.Roles)
            .NotEmpty().WithMessage("Role names cannot be empty.")
            .Must(BeAValidRoleName)
            .WithMessage(role => $"'{role}' is not a valid system role. Allowed roles: {string.Join(", ", Enum.GetNames<UserRoleEnum>())}.");
    }

    /// <summary>
    /// Validates that a string role name corresponds strictly to a defined <see cref="UserRoleEnum"/> member.
    /// </summary>
    private static bool BeAValidRoleName(string roleName)
    {
        return Enum.TryParse<UserRoleEnum>(roleName, ignoreCase: true, out _);
    }
}
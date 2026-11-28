namespace Ecommerce.Carts.Application.Validators;

using Ecommerce.Carts.Application.DTOs.Request;
using FluentValidation;

public class AddCartItemValidator : AbstractValidator<AddCartItemRequest>
{
    public AddCartItemValidator()
    {
        RuleFor(x => x.VariantId)
            .GreaterThan(0)
            .WithMessage("VariantId must be a valid positive identifier.");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 50)
            .WithMessage("Quantity must be between 1 and 50 units.");
    }
}

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemRequest>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 50)
            .WithMessage("Quantity must be between 1 and 50 units.");
    }
}

public class MergeCartValidator : AbstractValidator<MergeCartRequest>
{
    public MergeCartValidator()
    {
        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("Items list cannot be null.");

        RuleForEach(x => x.Items)
            .SetValidator(new AddCartItemValidator());
    }
}
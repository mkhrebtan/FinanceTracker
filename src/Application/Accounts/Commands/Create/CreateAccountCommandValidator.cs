using FluentValidation;

namespace Application.Accounts.Commands.Create;

public sealed class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Currency)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Length(3, 3)
            .WithMessage("Currency must be exactly 3 characters long.");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Amount.HasValue)
            .WithMessage("Amount must be greater than or equal to zero.");
    }
}

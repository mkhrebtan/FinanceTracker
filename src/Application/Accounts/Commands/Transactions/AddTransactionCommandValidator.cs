using Domain.Entities;
using FluentValidation;

namespace Application.Accounts.Commands.Transactions;

public abstract class AddTransactionCommandValidator : AbstractValidator<AddTransactionCommand>
{
    protected AddTransactionCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID cannot be empty.")
            .Must(BeValidGuid).WithMessage("Account ID must be a valid GUID.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category cannot be empty.")
            .MaximumLength(Transaction.MaxCategoryLength).WithMessage($"Category cannot exceed {Transaction.MaxCategoryLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(Transaction.MaxDescriptionLength).WithMessage($"Description cannot exceed {Transaction.MaxDescriptionLength} characters.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date cannot be empty.")
            .Must(BeValidDate).WithMessage($"Date must be between {Transaction.MinDate:yyyy-MM-dd} and {DateTime.UtcNow:yyyy-MM-dd}.");
    }

    private static bool BeValidGuid(Guid accountId)
    {
        return accountId != Guid.Empty;
    }

    private static bool BeValidDate(DateTime date)
    {
        return date >= Transaction.MinDate && date <= DateTime.UtcNow;
    }
}

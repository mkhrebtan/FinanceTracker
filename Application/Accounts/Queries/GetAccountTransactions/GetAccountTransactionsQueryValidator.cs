using FluentValidation;

namespace Application.Accounts.Queries.GetAccountTransactions;

internal sealed class GetAccountTransactionsQueryValidator : AbstractValidator<GetAccountTransactionsQuery>
{
    public GetAccountTransactionsQueryValidator()
    {
        RuleFor(query => query.AccountId)
            .NotEmpty().WithMessage("Account ID must not be empty.")
            .Must(BeValidGuid).WithMessage("Account ID must be a valid GUID.");
    }

    private static bool BeValidGuid(Guid accountId)
    {
        return accountId != Guid.Empty;
    }
}
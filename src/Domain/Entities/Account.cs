using Domain.Abstraction;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Account : AggregateRoot
{
    private readonly HashSet<Transaction> _transactions = new();

    private Account(Guid id, Money balance)
        : base(id)
    {
        Balance = balance;
    }

    public Money Balance { get; private set; }

    public IReadOnlyCollection<Transaction> Transactions => _transactions.ToList().AsReadOnly();

    public static Result<Account> Create(string currency)
    {
        var initialBalance = Money.Zero(currency);
        if (initialBalance.IsFailure)
        {
            return Result<Account>.Failure(initialBalance.Error);
        }

        return Result<Account>.Success(new Account(Guid.NewGuid(), initialBalance.Value));
    }

    public static Result<Account> Create(Money initialBalance)
    {
        if (initialBalance is null)
        {
            return Result<Account>.Failure(new Error("Account.InitialBalanceNull", "Initial balance cannot be null."));
        }

        return Result<Account>.Success(new Account(Guid.NewGuid(), initialBalance));
    }

    public Result<Transaction> AddExpense(Money amount, DateTime date, string category = Transaction.DefaultCategory, string description = Transaction.DefaultDescription)
    {
        if(!IsValidCurrency(amount.Currency))
        {
            return Result<Transaction>.Failure(new Error("Account.InvalidCurrency", "The currency of the amount does not match the account's currency."));
        }

        Result<Transaction> expenseResult = Expense.Create(amount, date, category, description);
        if (expenseResult.IsFailure)
        {
            return expenseResult;
        }

        Result<Money> newBalanceResult = Money.Create(Balance.Value - expenseResult.Value.Amount.Value, Balance.Currency);
        if (newBalanceResult.IsFailure)
        {
            return Result<Transaction>.Failure(newBalanceResult.Error);
        }

        _transactions.Add(expenseResult.Value);
        Balance = newBalanceResult.Value;
        return Result<Transaction>.Success(expenseResult.Value);
    }

    public Result<Transaction> AddIncome(Money amount, DateTime date, string category = Transaction.DefaultCategory, string description = Transaction.DefaultDescription)
    {
        if (!IsValidCurrency(amount.Currency))
        {
            return Result<Transaction>.Failure(new Error("Account.InvalidCurrency", "The currency of the amount does not match the account's currency."));
        }

        Result<Transaction> incomeResult = Income.Create(amount, date, category, description);
        if (incomeResult.IsFailure)
        {
            return incomeResult;
        }

        Result<Money> newBalanceResult = Money.Create(Balance.Value + incomeResult.Value.Amount.Value, Balance.Currency);
        if (newBalanceResult.IsFailure)
        {
            return Result<Transaction>.Failure(newBalanceResult.Error);
        }

        _transactions.Add(incomeResult.Value);
        Balance = newBalanceResult.Value;
        return Result<Transaction>.Success(incomeResult.Value);
    }

    private bool IsValidCurrency(string currency)
    {
        return Balance.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase);
    }
}

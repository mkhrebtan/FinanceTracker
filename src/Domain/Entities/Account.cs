using Domain.Abstraction;
using Domain.Abstraction.Interfaces;
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
        return AddTransaction<Expense>(amount, date, category, description);
    }

    public Result<Transaction> AddIncome(Money amount, DateTime date, string category = Transaction.DefaultCategory, string description = Transaction.DefaultDescription)
    {
        return AddTransaction<Income>(amount, date, category, description);
    }

    private Result<Transaction> AddTransaction<T>(Money amount, DateTime date, string category, string description)
        where T : Transaction, ICreatableTransaction
    {
        if (amount is null)
        {
            return Result<Transaction>.Failure(new Error("Account.AmountNull", "Amount cannot be null."));
        }

        if (!IsValidCurrency(amount.Currency))
        {
            return Result<Transaction>.Failure(new Error("Account.InvalidCurrency", "The currency of the amount does not match the account's currency."));
        }

        Result<Money> newBalanceResult = default!;
        if (typeof(T) == typeof(Expense))
        {
            newBalanceResult = Money.Create(Balance.Value - amount.Value, Balance.Currency);
        }
        else if (typeof(T) == typeof(Income))
        {
            newBalanceResult = Money.Create(Balance.Value + amount.Value, Balance.Currency);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported transaction type: {typeof(T).Name}");
        }

        if (newBalanceResult.IsFailure)
        {
            return Result<Transaction>.Failure(newBalanceResult.Error);
        }

        Result<Transaction> transactionResult = T.Create(amount, date, category, description);
        if (transactionResult.IsFailure)
        {
            return transactionResult;
        }

        Balance = newBalanceResult.Value;
        _transactions.Add(transactionResult.Value);
        return transactionResult;
    }

    private bool IsValidCurrency(string currency)
    {
        return Balance.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase);
    }
}

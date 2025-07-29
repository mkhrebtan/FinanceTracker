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

    public Result<Transaction> AddExpense(Money amount, DateTime date, string category = Transaction.DefaultCategory, string description = Transaction.DefaultDescription)
    {
        Result<Transaction> expenseResult = Expense.Create(amount, date, category, description);
        if (expenseResult.IsFailure)
        {
            return expenseResult;
        }

        Transaction expense = expenseResult.Value;
        Money expenseAmount = expense.Amount;
        Result<Money> newBalanceResult = Money.Create(Balance.Value - expenseAmount.Value, Balance.Currency);
        if (newBalanceResult.IsFailure)
        {
            return Result<Transaction>.Failure(newBalanceResult.Error);
        }

        _transactions.Add(expense);
        Balance = newBalanceResult.Value;
        return Result<Transaction>.Success(expense);
    }

    public Result<Transaction> AddIncome(Money amount, DateTime date, string category = Transaction.DefaultCategory, string description = Transaction.DefaultDescription)
    {
        Result<Transaction> incomeResult = Income.Create(amount, date, category, description);
        if (incomeResult.IsFailure)
        {
            return incomeResult;
        }

        Transaction income = incomeResult.Value;
        Money incomeAmount = income.Amount;
        Result<Money> newBalanceResult = Money.Create(Balance.Value + incomeAmount.Value, Balance.Currency);
        if (newBalanceResult.IsFailure)
        {
            return Result<Transaction>.Failure(newBalanceResult.Error);
        }

        _transactions.Add(income);
        Balance = newBalanceResult.Value;
        return Result<Transaction>.Success(income);
    }
}

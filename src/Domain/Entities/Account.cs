using Domain.Abstraction;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Account : AggregateRoot
{
    private HashSet<Transaction> _transactions = new();

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

    public Result<Expense> AddExpense(Money amount, DateTime date, string category = Transaction.DefaultCategory, string description = Transaction.DefaultDescription)
    {
        Result<Expense> expenseResult = Expense.Create(amount, date, category, description);
        if (expenseResult.IsFailure)
        {
            return expenseResult;
        }

        Expense expense = expenseResult.Value;
        Money expenseAmount = expense.Amount;
        Result<Money> newBalanceResult = Money.Create(Balance.Value - expenseAmount.Value, Balance.Currency);
        if (newBalanceResult.IsFailure)
        {
            return Result<Expense>.Failure(newBalanceResult.Error);
        }

        _transactions.Add(expense);
        Balance = newBalanceResult.Value;
        return Result<Expense>.Success(expense);
    }

    public Result<Income> AddIncome(Money amount, DateTime date, string category = Transaction.DefaultCategory, string description = Transaction.DefaultDescription)
    {
        Result<Income> incomeResult = Income.Create(amount, date, category, description);
        if (incomeResult.IsFailure)
        {
            return incomeResult;
        }

        Income income = incomeResult.Value;
        Money incomeAmount = income.Amount;
        Result<Money> newBalanceResult = Money.Create(Balance.Value + incomeAmount.Value, Balance.Currency);
        if (newBalanceResult.IsFailure)
        {
            return Result<Income>.Failure(newBalanceResult.Error);
        }

        _transactions.Add(income);
        Balance = newBalanceResult.Value;
        return Result<Income>.Success(income);
    }
}

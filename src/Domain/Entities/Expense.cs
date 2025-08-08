using Domain.Abstraction.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Expense : Transaction, ICreatableTransaction
{
    private Expense(Guid id, Money amount, string category, DateTime date, string description)
        : base(id, amount, category, date, description)
    {
    }

    public static Result<Transaction> Create(Money amount, DateTime date, string category, string description)
    {
        var validationResult = ValidateTransaction(amount, date, category, description);
        if (!validationResult.IsSuccess)
        {
            return Result<Transaction>.Failure(validationResult.Error);
        }

        var expense = new Expense(Guid.NewGuid(), amount, category, date, description);
        return Result<Transaction>.Success(expense);
    }
}

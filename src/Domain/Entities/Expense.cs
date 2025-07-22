using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Expense : Transaction
{
    private Expense(Guid id, Money amount, string category, DateTime date, string description)
        : base(id)
    {
        Amount = amount;
        Category = category;
        Date = date;
        Description = description;
    }

    public static Result<Expense> Create(Money amount, DateTime date, string category = DefaultCategory, string description = DefaultDescription)
    {
        var validationResult = ValidateTransaction(amount, date, category, description);
        if (!validationResult.IsSuccess)
        {
            return Result<Expense>.Failure(validationResult.Error);
        }

        var expense = new Expense(Guid.NewGuid(), amount, category, date, description);
        return Result<Expense>.Success(expense);
    }
}

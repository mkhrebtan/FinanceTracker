using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Income : Transaction
{
    private Income(Guid id, Money amount, string category, DateTime date, string description)
        : base(id, amount, category, date, description)
    {
    }

    public static Result<Transaction> Create(Money amount, DateTime date, string category = DefaultCategory, string description = DefaultDescription)
    {
        var validationResult = ValidateTransaction(amount, date, category, description);
        if (!validationResult.IsSuccess)
        {
            return Result<Transaction>.Failure(validationResult.Error);
        }

        var income = new Income(Guid.NewGuid(), amount, category, date, description);
        return Result<Transaction>.Success(income);
    }
}

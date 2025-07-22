using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Income : Transaction
{
    private Income(Guid id, Money amount, string category, DateTime date, string description)
        : base(id)
    {
        Amount = amount;
        Category = category;
        Date = date;
        Description = description;
    }

    public static Result<Income> Create(Money amount, DateTime date, string category = DefaultCategory, string description = "")
    {
        var validationResult = ValidateTransaction(amount, date, category, description);
        if (!validationResult.IsSuccess)
        {
            return Result<Income>.Failure(validationResult.Error);
        }

        var income = new Income(Guid.NewGuid(), amount, category, date, description);
        return Result<Income>.Success(income);
    }
}

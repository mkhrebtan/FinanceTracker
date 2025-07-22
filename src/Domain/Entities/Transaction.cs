using Domain.Abstraction;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities;

public abstract class Transaction : Entity
{
    public const string DefaultCategory = "default";

    public const string DefaultDescription = "No description provided";

    public const int MaxCategoryLength = 50;

    public const int MaxDescriptionLength = 1000;

    protected Transaction(Guid id)
        : base(id)
    {
    }

    public static DateTime MinDate => new DateTime(2000, 1, 1).ToUniversalTime();

    public Money Amount { get; protected set; }

    public string Category { get; protected set; }

    public DateTime Date { get; protected set; }

    public string Description { get; protected set; }

    protected static bool IsValidCategory(string category)
    {
        return !string.IsNullOrWhiteSpace(category) && category.Length <= MaxCategoryLength;
    }

    protected static bool IsValidDescription(string description)
    {
        return description.Length <= MaxDescriptionLength;
    }

    protected static bool IsValidDate(DateTime date)
    {
        return date >= MinDate && date <= DateTime.UtcNow;
    }

    protected static bool IsValidAmount(Money amount)
    {
        return amount.Value > 0;
    }

    protected static Result ValidateTransaction(Money amount, DateTime date, string category, string description)
    {
        if (!IsValidAmount(amount))
        {
            return Result.Failure(new Error("Transaction.InvalidAmount", "Amount must be greater than zero."));
        }

        if (!IsValidDate(date))
        {
            return Result.Failure(new Error("Transaction.InvalidDate", $"Date must be between {MinDate:yyyy-MM-dd} and {DateTime.UtcNow:yyyy-MM-dd}."));
        }

        if (!IsValidCategory(category))
        {
            return Result.Failure(new Error("Transaction.InvalidCategory", $"Category must be a non-empty string with a maximum length of {MaxCategoryLength} characters."));
        }

        if (!IsValidDescription(description))
        {
            return Result.Failure(new Error("Transaction.InvalidDescription", $"Description must not exceed {MaxDescriptionLength} characters."));
        }

        return Result.Success();
    }
}
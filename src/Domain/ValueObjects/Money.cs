using Domain.Abstraction;
using Domain.Shared;

namespace Domain.ValueObjects;

public class Money : ValueObject
{
    private Money(decimal value, string currency)
    {
        Value = value;
        Currency = currency;
    }

    public decimal Value { get; }

    public string Currency { get; }

    public static Result<Money> Create(decimal value, string currency)
    {
        if (!IsValidCurrency(currency))
        {
            return Result<Money>.Failure(new Error("Money.EmptyCurrency", "Currency cannot be null or empty."));
        }

        if (value < 0)
        {
            return Result<Money>.Failure(new Error("Money.NegativeValue", "Money amount cannot be negative."));
        }

        return Result<Money>.Success(new Money(value, currency));
    }

    public static Result<Money> Zero(string currency)
    {
        return Create(0, currency);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
        yield return Currency;
    }

    private static bool IsValidCurrency(string currency)
    {
        // Implement currency validation logic here, e.g., check against a list of valid currencies.
        return !string.IsNullOrWhiteSpace(currency);
    }
}

using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Theory]
    [MemberData(nameof(ValidMoneyData))]
    public void Create_WithValidValues_ShouldReturnSuccess(decimal amount, string currency) 
    {
        var result = Money.Create(amount, currency);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(amount, result.Value.Value);
        Assert.Equal(currency, result.Value.Currency);
    }

    [Theory]
    [MemberData(nameof(InvalidMoneyData))]
    public void Create_WithInvalidValues_ShouldReturnFailure(decimal amount, string currency)
    {
        var result = Money.Create(amount, currency);
        Assert.True(result.IsFailure);
        Assert.NotEqual(result.Error, Error.None);
    }

    public static TheoryData<decimal, string> ValidMoneyData => new()
    {
        { 100.00m, "USD" },
        { 50.50m, "EUR" },
        { 0.00m, "GBP" },
        { 9999.99m, "JPY" }
    };

    public static TheoryData<decimal, string> InvalidMoneyData => new()
    {
        { -50.00m, "USD" },
        { 100.00m, string.Empty },
        { 99.90m, null! },
    };
}

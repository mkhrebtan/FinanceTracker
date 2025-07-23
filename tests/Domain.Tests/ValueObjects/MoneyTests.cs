using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Theory]
    [MemberData(nameof(MoneyData.ValidMoneyData), MemberType = typeof(MoneyData))]
    public void Create_WithValidValues_ShouldReturnSuccess(decimal amount, string currency) 
    {
        var result = Money.Create(amount, currency);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(amount, result.Value.Value);
        Assert.Equal(currency, result.Value.Currency);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldReturnFailure()
    {
        var result = Money.Create(-100.00m, "USD");
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithEmptyCurrency_ShouldReturnFailure()
    {
        var result = Money.Create(100.00m, string.Empty);
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }
}

public class MoneyData
{
    public static IEnumerable<object[]> ValidMoneyData()
    {
        yield return new object[] { 100.50m, "UAH" };
        yield return new object[] { 200.00m, "USD" };
        yield return new object[] { 0.00m, "EUR" };
    }
}

using Domain.Entities;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Tests.Entities;

public abstract class TransactionTests
{
    public delegate Result<Transaction> TransactionFactory(
        Money amount,
        DateTime date,
        string category = Transaction.DefaultCategory,
        string description = Transaction.DefaultDescription);

    public TransactionFactory Factory;

    protected TransactionTests(TransactionFactory factory)
    {
        Factory = factory;
    }

    [Theory]
    [MemberData(nameof(InvalidAmounts))]
    public void Create_WithInvalidAmount_ShouldReturnFailure(Money amount)
    {
        DateTime date = DateTime.UtcNow;

        var result = Factory(amount, date);

        Assert.True(result.IsFailure);
        Assert.NotEqual(result.Error, Error.None);
    }

    [Theory]
    [MemberData(nameof(InvalidExpenseDates))]
    public void Create_WithInvalidDate_ShouldReturnFailure(DateTime date)
    {
        Money amount = Money.Create(100.00m, "USD").Value;
        var result = Factory(amount, date);
        Assert.True(result.IsFailure);
        Assert.NotEqual(result.Error, Error.None);
    }

    [Theory]
    [MemberData(nameof(InvalidCategories))]
    public void Create_WithInvalidCategory_ShouldReturnFailure(string invalidCategory)
    {
        Money amount = Money.Create(100.00m, "USD").Value;
        DateTime date = DateTime.UtcNow;
        var result = Factory(amount, date, invalidCategory);
        Assert.True(result.IsFailure);
        Assert.NotEqual(result.Error, Error.None);
    }

    [Theory]
    [MemberData(nameof(InvalidDescriptions))]
    public void Create_WithInvalidDescription_ShouldReturnFailure(string invalidDesc)
    {
        Money amount = Money.Create(100.00m, "USD").Value;
        DateTime date = DateTime.UtcNow;
        var result = Factory(amount, date, description:invalidDesc);
        Assert.True(result.IsFailure);
        Assert.NotEqual(result.Error, Error.None);
    }

    [Theory]
    [MemberData(nameof(ValidExpenseData))]
    public void Create_WithValidData_ShouldReturnSuccess(decimal amount, DateTime date, string category, string description)
    {
        var money = Money.Create(amount, "USD").Value;
        var result = Factory(money, date, category, description);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    #region Data
    public static TheoryData<Money> InvalidAmounts => new()
    {
        Money.Zero("USD").Value, // Zero amount
        Money.Create(-50.00m, "__").Value,
        Money.Create(0.00m, "USD").Value // Zero amount
    };

    public static TheoryData<DateTime> InvalidExpenseDates => new()
    {
        { Transaction.MinDate.AddYears(-5) }, // Before MinDate
        { Transaction.MinDate.AddDays(-1) }, // Just before MinDate
        { DateTime.UtcNow.AddDays(1) }, // Future date
        { DateTime.MaxValue } // Max date
    };

    public static TheoryData<string> InvalidCategories =>
    [
        new string('a', Transaction.MaxCategoryLength + 1), // Exceeding max length
        string.Empty, // Empty category
        null! // Null category
    ];

    public static TheoryData<string> InvalidDescriptions =>
    [
        new string('a', Transaction.MaxDescriptionLength + 1), // Exceeding max length
        null! // Null category
    ];

    public static TheoryData<decimal, DateTime, string, string> ValidExpenseData => new()
    {
        { 100.00m, DateTime.UtcNow, "Food", "Lunch at restaurant" },
        { 50.50m, DateTime.UtcNow.AddDays(-1), "Transport", "Taxi fare" },
        { 0.01m, DateTime.UtcNow.AddDays(-2), "Utilities", "Electricity bill" },
        { 9999.99m, DateTime.UtcNow.AddDays(-3), "Entertainment", "Concert tickets" }
    };
    #endregion
}

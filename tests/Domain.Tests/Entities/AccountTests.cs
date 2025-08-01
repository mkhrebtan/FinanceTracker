using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Tests.Entities;

public class AccountTests
{
    #region Create Tests - With Currency String

    [Fact]
    public void Create_WithValidCurrency_ShouldReturnSuccessWithZeroBalance()
    {
        // Arrange
        string currency = "USD";

        // Act
        var result = Account.Create(currency);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Balance.Equals(Money.Zero(currency).Value));
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("JPY")]
    public void Create_WithDifferentValidCurrencies_ShouldReturnSuccess(string currency)
    {
        // Act
        var result = Account.Create(currency);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Balance.Equals(Money.Zero(currency).Value));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidCurrency_ShouldReturnFailure(string invalidCurrency)
    {
        // Act
        var result = Account.Create(invalidCurrency);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullCurrency_ShouldReturnFailure()
    {
        // Act
        var result = Account.Create((string)null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Create Tests - With Money Object

    [Fact]
    public void Create_WithValidMoney_ShouldReturnSuccessWithCorrectBalance()
    {
        // Arrange
        var initialBalance = Money.Create(500.50m, "USD").Value;

        // Act
        var result = Account.Create(initialBalance);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Balance.Equals(initialBalance));
    }

    [Fact]
    public void Create_WithZeroMoney_ShouldReturnSuccess()
    {
        // Arrange
        var zeroBalance = Money.Zero("EUR").Value;

        // Act
        var result = Account.Create(zeroBalance);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(zeroBalance.Equals(result.Value.Balance));
    }

    [Fact]
    public void Create_WithNullMoney_ShouldReturnFailure()
    {
        // Act
        var result = Account.Create((Money)null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region AddExpense Tests

    [Fact]
    public void AddExpense_WithValidExpense_ShouldUpdateBalanceAndAddTransaction()
    {
        // Arrange
        var initialBalance = Money.Create(1000.00m, "USD").Value;
        var account = Account.Create(initialBalance).Value;
        var expenseAmount = Money.Create(150.75m, "USD").Value;
        var expectedBalance = Money.Create(849.25m, "USD").Value; // 1000 - 150.75
        var date = DateTime.UtcNow.AddHours(-1);

        // Act
        var result = account.AddExpense(expenseAmount, date, "Food", "Grocery shopping");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.IsType<Expense>(result.Value);
        Assert.True(account.Balance.Equals(expectedBalance)); // 1000 - 150.75
        Assert.Single(account.Transactions);
        Assert.Contains(result.Value, account.Transactions);
    }

    [Fact]
    public void AddExpense_MultipleExpenses_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var initialBalance = Money.Create(500.00m, "USD").Value;
        var account = Account.Create(initialBalance).Value;
        var expense1 = Money.Create(100.00m, "USD").Value;
        var expense2 = Money.Create(75.50m, "USD").Value;
        var expectedBalance = Money.Create(324.50m, "USD").Value; // 500 - 100 - 75.50

        // Act
        var result1 = account.AddExpense(expense1, DateTime.UtcNow);
        var result2 = account.AddExpense(expense2, DateTime.UtcNow);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.True(account.Balance.Equals(expectedBalance));
        Assert.Equal(2, account.Transactions.Count);
        Assert.Contains(result1.Value, account.Transactions);
        Assert.Contains(result2.Value, account.Transactions);
    }

    [Fact]
    public void AddExpense_WhenResultingInNegativeBalance_ShouldReturnFailure()
    {
        // Arrange
        var initialBalance = Money.Create(100.00m, "USD").Value;
        var account = Account.Create(initialBalance).Value;
        var largeExpense = Money.Create(150.00m, "USD").Value;

        // Act
        var result = account.AddExpense(largeExpense, DateTime.UtcNow);

        // Assert
        Assert.True(result.IsFailure);
        Assert.True(account.Balance.Equals(initialBalance));
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void AddExpense_WithDifferentCurrency_ShouldReturnFailure()
    {
        // Arrange
        var account = Account.Create("USD").Value;
        var expenseInDifferentCurrency = Money.Create(50.00m, "EUR").Value;

        // Act
        var result = account.AddExpense(expenseInDifferentCurrency, DateTime.UtcNow);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Empty(account.Transactions);
        Assert.Equal(0m, account.Balance.Value);
    }

    #endregion

    #region AddIncome Tests

    [Fact]
    public void AddIncome_WithValidIncome_ShouldUpdateBalanceAndAddTransaction()
    {
        // Arrange
        var initialBalance = Money.Create(100.00m, "USD").Value;
        var account = Account.Create(initialBalance).Value;
        var incomeAmount = Money.Create(250.75m, "USD").Value;
        var expectedBalance = Money.Create(350.75m, "USD").Value; // 100 + 250.75
        var date = DateTime.UtcNow.AddHours(-2);

        // Act
        var result = account.AddIncome(incomeAmount, date, "Salary", "Monthly salary");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.IsType<Income>(result.Value);
        Assert.True(account.Balance.Equals(expectedBalance));
        Assert.Single(account.Transactions);
    }

    [Fact]
    public void AddIncome_MultipleIncomes_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var account = Account.Create("USD").Value;
        var income1 = Money.Create(500.00m, "USD").Value;
        var income2 = Money.Create(300.25m, "USD").Value;
        var expectedBalance = Money.Create(800.25m, "USD").Value; // 0 + 500 + 300.25

        // Act
        var result1 = account.AddIncome(income1, DateTime.UtcNow);
        var result2 = account.AddIncome(income2, DateTime.UtcNow);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.True(account.Balance.Equals(expectedBalance));
        Assert.Equal(2, account.Transactions.Count);
        Assert.Contains(result1.Value, account.Transactions);
        Assert.Contains(result2.Value, account.Transactions);
    }

    [Fact]
    public void AddIncome_WithDifferentCurrency_ShouldReturnFailure()
    {
        // Arrange
        var account = Account.Create("USD").Value;
        var incomeInDifferentCurrency = Money.Create(100.00m, "EUR").Value;

        // Act
        var result = account.AddIncome(incomeInDifferentCurrency, DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Empty(account.Transactions);
        Assert.Equal(0m, account.Balance.Value);
    }

    #endregion

    #region Mixed Transaction Tests

    [Fact]
    public void Account_WithMixedTransactions_ShouldMaintainCorrectBalance()
    {
        // Arrange
        var initialBalance = Money.Create(1000.00m, "USD").Value;
        var account = Account.Create(initialBalance).Value;

        // Act & Assert
        // Add income
        var income = account.AddIncome(Money.Create(500.00m, "USD").Value, DateTime.UtcNow, "Bonus", "Year-end bonus");
        Assert.True(income.IsSuccess);
        Assert.Equal(1500.00m, account.Balance.Value);

        // Add expense
        var expense = account.AddExpense(Money.Create(200.00m, "USD").Value, DateTime.UtcNow, "Rent", "Monthly rent");
        Assert.True(expense.IsSuccess);
        Assert.Equal(1300.00m, account.Balance.Value);

        // Add another income
        var income2 = account.AddIncome(Money.Create(150.75m, "USD").Value, DateTime.UtcNow, "Freelance", "Side project");
        Assert.True(income2.IsSuccess);
        Assert.Equal(1450.75m, account.Balance.Value);

        // Add another expense
        var expense2 = account.AddExpense(Money.Create(75.25m, "USD").Value, DateTime.UtcNow, "Food", "Dinner");
        Assert.True(expense2.IsSuccess);
        Assert.Equal(1375.50m, account.Balance.Value);

        // Verify transaction count
        Assert.Equal(4, account.Transactions.Count);
    }

    #endregion
}

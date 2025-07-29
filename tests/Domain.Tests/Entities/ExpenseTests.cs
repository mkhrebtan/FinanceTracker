using Domain.Entities;

namespace Domain.Tests.Entities;

public class ExpenseTests : TransactionTests
{
    public ExpenseTests()
        : base(new TransactionFactory(Expense.Create))
    {
    }
}
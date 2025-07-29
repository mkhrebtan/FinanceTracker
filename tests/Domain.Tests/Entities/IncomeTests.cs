using Domain.Entities;

namespace Domain.Tests.Entities;

public class IncomeTests : TransactionTests
{
    public IncomeTests()
        : base(new TransactionFactory(Income.Create))
    {
    }
}

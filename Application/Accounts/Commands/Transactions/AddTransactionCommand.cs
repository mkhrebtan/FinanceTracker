namespace Application.Accounts.Commands.Transactions;

public abstract record AddTransactionCommand(Guid AccountId, decimal Amount, DateTime Date, string Category, string Description)
{
}

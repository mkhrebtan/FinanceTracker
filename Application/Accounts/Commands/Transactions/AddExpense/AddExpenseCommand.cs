using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Accounts.Commands.Transactions.AddExpense;

public sealed record AddExpenseCommand : AddTransactionCommand, ICommand<TransactionDto>
{
    public AddExpenseCommand(Guid accountId, decimal amount, DateTime date, string category, string description)
        : base(accountId, amount, date, category, description)
    {
    }
}
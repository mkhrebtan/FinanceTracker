using Application.Accounts.Commands.Transactions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Accounts.Commands.Transactions.AddIncome;

public sealed record AddIncomeCommand
    : AddTransactionCommand, ICommand<TransactionDto>
{
    public AddIncomeCommand(Guid accountId, decimal amount, DateTime date, string category, string description)
        : base(accountId, amount, date, category, description)
    {
    }
}
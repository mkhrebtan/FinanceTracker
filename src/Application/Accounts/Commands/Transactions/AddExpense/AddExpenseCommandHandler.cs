using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Repos;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Accounts.Commands.Transactions.AddExpense;

internal sealed class AddExpenseCommandHandler : AddTransactionCommandHandler, ICommandHandler<AddExpenseCommand, TransactionDto>
{
    public AddExpenseCommandHandler(IAccountRepository accountRepository, ILogger<AddExpenseCommandHandler> logger)
        : base(accountRepository, logger)
    {
    }

    public async Task<Result<TransactionDto>> Handle(AddExpenseCommand command)
    {
        Logger.LogInformation(
            "Handling AddExpenseCommand for AccountId: {AccountId}, Amount: {Amount}, Date: {Date}, Category: {Category}, Description: {Description}",
            command.AccountId,
            command.Amount,
            command.Date,
            command.Category,
            command.Description);

        Account? account = AccountRepository.GetById(command.AccountId);
        if (account is null)
        {
            Logger.LogWarning("Account with ID {AccountId} not found.", command.AccountId);
            return Result<TransactionDto>.Failure(new Error("Account.NotFound", "The specified account does not exist."));
        }

        return await Handle(command, account.Balance.Currency, account.AddExpense);
    }
}
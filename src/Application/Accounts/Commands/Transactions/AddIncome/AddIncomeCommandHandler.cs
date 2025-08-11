using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Repos;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Accounts.Commands.Transactions.AddIncome;

internal sealed class AddIncomeCommandHandler : AddTransactionCommandHandler, ICommandHandler<AddIncomeCommand, TransactionDto>
{
    public AddIncomeCommandHandler(IAccountRepository accountRepository, ILogger<AddIncomeCommandHandler> logger)
        : base(accountRepository, logger)
    {
    }

    public async Task<Result<TransactionDto>> Handle(AddIncomeCommand command)
    {
        Logger.LogInformation(
            "Handling AddIncomeCommand for AccountId: {AccountId}, Amount: {Amount}, Date: {Date}, Category: {Category}, Description: {Description}",
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

        return await Handle(command, account.Balance.Currency, account.AddIncome);
    }
}

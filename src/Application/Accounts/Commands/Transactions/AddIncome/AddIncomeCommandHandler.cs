using Application.Accounts.Commands.Transactions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Repos;
using Domain.Shared;

namespace Application.Accounts.Commands.Transactions.AddIncome;

internal sealed class AddIncomeCommandHandler : AddTransactionCommandHandler, ICommandHandler<AddIncomeCommand, TransactionDto>
{
    public AddIncomeCommandHandler(IAccountRepository accountRepository)
        : base(accountRepository)
    {
    }

    public async Task<Result<TransactionDto>> Handle(AddIncomeCommand command)
    {
        Account? account = AccountRepository.GetById(command.AccountId);
        if (account is null)
        {
            return Result<TransactionDto>.Failure(new Error("Account.NotFound", "The specified account does not exist."));
        }

        return await Handle(command, account.Balance.Currency, account.AddIncome);
    }
}

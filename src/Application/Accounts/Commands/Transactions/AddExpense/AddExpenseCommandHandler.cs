using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Repos;
using Domain.Shared;

namespace Application.Accounts.Commands.Transactions.AddExpense;

internal sealed class AddExpenseCommandHandler : AddTransactionCommandHandler, ICommandHandler<AddExpenseCommand, TransactionDto>
{
    public AddExpenseCommandHandler(IAccountRepository accountRepository)
        : base(accountRepository)
    {
    }

    public async Task<Result<TransactionDto>> Handle(AddExpenseCommand command)
    {
        Account? account = AccountRepository.GetById(command.AccountId);
        if (account is null)
        {
            return Result<TransactionDto>.Failure(new Error("Account.NotFound", "The specified account does not exist."));
        }

        return await Handle(command, account.Balance.Currency, account.AddExpense);
    }
}
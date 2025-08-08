using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Repos;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Accounts.Commands.Transactions;

internal abstract class AddTransactionCommandHandler
{
    protected AddTransactionCommandHandler(IAccountRepository accountRepository)
    {
        AccountRepository = accountRepository;
    }

    protected IAccountRepository AccountRepository { get; }

    protected async Task<Result<TransactionDto>> Handle<TCommand>(TCommand command, string accountCurrency, Func<Money, DateTime, string, string, Result<Transaction>> AddTransaction)
        where TCommand : AddTransactionCommand, ICommand<TransactionDto>
    {
        Result<Money> amountResult = Money.Create(command.Amount, accountCurrency);
        if (amountResult.IsFailure)
        {
            return Result<TransactionDto>.Failure(amountResult.Error);
        }

        Result<Transaction> transactionResult = AddTransaction(
            amountResult.Value,
            command.Date,
            command.Category,
            command.Description);

        if (transactionResult.IsFailure)
        {
            return Result<TransactionDto>.Failure(transactionResult.Error);
        }

        // TODO: Save changes to the repository

        return await Task.FromResult(Result<TransactionDto>.Success(new TransactionDto
        {
            Id = transactionResult.Value.Id,
            Amount = transactionResult.Value.Amount.Value,
            Currency = transactionResult.Value.Amount.Currency,
            Category = transactionResult.Value.Category,
            Date = transactionResult.Value.Date,
            Description = transactionResult.Value.Description,
        }));
    }
}
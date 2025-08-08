using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Repos;
using Domain.Shared;

namespace Application.Accounts.Queries.GetAccountTransactions;

internal sealed class GetAccountTransactionsQueryHandler : IQueryHandler<GetAccountTransactionsQuery, IReadOnlyCollection<TransactionDto>>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountTransactionsQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<IReadOnlyCollection<TransactionDto>>> Handle(GetAccountTransactionsQuery request)
    {
        Account? account = _accountRepository.GetById(request.AccountId);
        if (account is null)
        {
            return Result<IReadOnlyCollection<TransactionDto>>.Failure(new Error("GetAccountTransactionsQuery.AccountNotFound", "The specified account does not exist."));
        }

        return await Task.FromResult(Result<IReadOnlyCollection<TransactionDto>>.Success(account.Transactions.Select(transaction => new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount.Value,
            Currency = transaction.Amount.Currency,
            Category = transaction.Category,
            Date = transaction.Date,
            Description = transaction.Description,
        }).ToList()));
    }
}

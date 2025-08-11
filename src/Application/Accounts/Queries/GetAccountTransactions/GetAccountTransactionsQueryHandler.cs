using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Repos;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Accounts.Queries.GetAccountTransactions;

internal sealed class GetAccountTransactionsQueryHandler : IQueryHandler<GetAccountTransactionsQuery, IReadOnlyCollection<TransactionDto>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<GetAccountTransactionsQueryHandler> _logger;

    public GetAccountTransactionsQueryHandler(IAccountRepository accountRepository, ILogger<GetAccountTransactionsQueryHandler> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<TransactionDto>>> Handle(GetAccountTransactionsQuery request)
    {
        _logger.LogInformation("Handling GetAccountTransactionsQuery for AccountId: {AccountId}", request.AccountId);

        Account? account = _accountRepository.GetById(request.AccountId);
        if (account is null)
        {
            _logger.LogWarning("Account with ID {AccountId} not found.", request.AccountId);
            return Result<IReadOnlyCollection<TransactionDto>>.Failure(new Error("GetAccountTransactionsQuery.AccountNotFound", "The specified account does not exist."));
        }

        _logger.LogInformation("Found {TransactionCount} transactions for AccountId: {AccountId}", account.Transactions.Count, request.AccountId);
        return await Task.FromResult(Result<IReadOnlyCollection<TransactionDto>>.Success(account.Transactions.Select(transaction => new TransactionDto
        {
            Id = transaction.Id,
            TransactionType = transaction.GetType().Name.ToLower(),
            Amount = transaction.Amount.Value,
            Currency = transaction.Amount.Currency,
            Category = transaction.Category,
            Date = transaction.Date,
            Description = transaction.Description,
        }).ToList()));
    }
}

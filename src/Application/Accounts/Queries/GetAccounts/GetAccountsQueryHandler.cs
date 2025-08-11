using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repos;
using Domain.Shared;

namespace Application.Accounts.Queries.GetAccounts;

internal sealed class GetAccountsQueryHandler : IQueryHandler<GetAccountsQuery, IReadOnlyCollection<AccountDto>>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountsQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<IReadOnlyCollection<AccountDto>>> Handle(GetAccountsQuery request)
    {
        var accounts = _accountRepository.GetAll();

        return await Task.FromResult(Result<IReadOnlyCollection<AccountDto>>.Success(accounts.Select(account => new AccountDto
        {
            Id = account.Id,
            Balance = account.Balance.Value,
            Currency = account.Balance.Currency,
        }).ToList()));
    }
}

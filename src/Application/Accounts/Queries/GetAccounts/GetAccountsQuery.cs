using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Accounts.Queries.GetAccounts;

public sealed record GetAccountsQuery : IQuery<IReadOnlyCollection<AccountDto>>
{
}
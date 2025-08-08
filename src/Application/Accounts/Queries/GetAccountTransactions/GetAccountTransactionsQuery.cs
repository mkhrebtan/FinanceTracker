using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Accounts.Queries.GetAccountTransactions;

public sealed record GetAccountTransactionsQuery(Guid AccountId) : IQuery<IReadOnlyCollection<TransactionDto>>
{
}
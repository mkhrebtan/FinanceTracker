using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Accounts.Commands.Create;

public sealed record CreateAccountCommand(string Currency, decimal? Amount = null)
    : ICommand<AccountDto>
{
}
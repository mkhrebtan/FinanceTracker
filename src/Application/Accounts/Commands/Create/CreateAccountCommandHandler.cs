using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Repos;
using Domain.Shared;
using Domain.ValueObjects;

namespace Application.Accounts.Commands.Create;

internal sealed class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;

    public CreateAccountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<AccountDto>> Handle(CreateAccountCommand command)
    {
        Result<Account> accountResult = default!;
        if (command.Amount is null)
        {
            accountResult = Account.Create(command.Currency);
        }
        else
        {
            Result<Money> initialBalance = Money.Create(command.Amount.Value, command.Currency);
            if (initialBalance.IsFailure)
            {
                return Result<AccountDto>.Failure(initialBalance.Error);
            }

            accountResult = Account.Create(initialBalance.Value);
        }

        if (accountResult.IsFailure)
        {
            return Result<AccountDto>.Failure(accountResult.Error);
        }

        _accountRepository.Insert(accountResult.Value);
        return await Task.FromResult(Result<AccountDto>.Success(new AccountDto
        {
            Id = accountResult.Value.Id,
            Balance = accountResult.Value.Balance.Value,
            Currency = accountResult.Value.Balance.Currency,
        }));
    }
}

using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Domain.Repos;
using Domain.Shared;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Accounts.Commands.Create;

internal sealed class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<CreateAccountCommandHandler> _logger;

    public CreateAccountCommandHandler(IAccountRepository accountRepository, ILogger<CreateAccountCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<Result<AccountDto>> Handle(CreateAccountCommand command)
    {
        _logger.LogInformation("Handling CreateAccountCommand for currency: {Currency}, amount: {Amount}", command.Currency, command.Amount);

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
                _logger.LogWarning("Failed to create initial balance: {Error}", initialBalance.Error);
                return Result<AccountDto>.Failure(initialBalance.Error);
            }

            accountResult = Account.Create(initialBalance.Value);
        }

        if (accountResult.IsFailure)
        {
            _logger.LogWarning("Failed to create account: {Error}", accountResult.Error);
            return Result<AccountDto>.Failure(accountResult.Error);
        }

        _accountRepository.Insert(accountResult.Value);
        _logger.LogInformation("Account created with ID: {AccountId}", accountResult.Value.Id);
        return await Task.FromResult(Result<AccountDto>.Success(new AccountDto
        {
            Id = accountResult.Value.Id,
            Balance = accountResult.Value.Balance.Value,
            Currency = accountResult.Value.Balance.Currency,
        }));
    }
}

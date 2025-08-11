using Api.Extensions;
using Api.Middlewares.Exceptions;
using Application;
using Application.Accounts.Commands.Create;
using Application.Accounts.Commands.Transactions;
using Application.Accounts.Commands.Transactions.AddExpense;
using Application.Accounts.Commands.Transactions.AddIncome;
using Application.Accounts.Queries.GetAccounts;
using Application.Accounts.Queries.GetAccountTransactions;
using Application.Common.Interfaces;
using Application.Common.Models;
using FluentValidation;
using Persistence;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Finance Tracker API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddOpenApi();

    builder.Services.AddPersistence();
    builder.Services.AddApplicationServices();

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration));

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Finance Tracker API";
        });
    }

    app.UseHttpsRedirection();

    app.MapGet("/accounts", async (IQueryHandler<GetAccountsQuery, IReadOnlyCollection<AccountDto>> queryHandler) =>
    {
        var query = new GetAccountsQuery();
        var accountsResult = await queryHandler.Handle(query);
        return accountsResult.IsSuccess ? Results.Ok(accountsResult.Value) : Results.Problem(accountsResult.Error.Message);
    });

    app.MapGet("/accounts/{accountId}/transactions", async (
        Guid accountId,
        IQueryHandler<GetAccountTransactionsQuery, IReadOnlyCollection<TransactionDto>> queryHandler,
        IValidator<GetAccountTransactionsQuery> validator,
        ILogger<GetAccountTransactionsQuery> logger) =>
    {
        var request = new GetAccountTransactionsQuery(accountId);
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogValidationFailure<GetAccountTransactionsQuery, IReadOnlyCollection<TransactionDto>>(validationResult, request);
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var transactionsResult = await queryHandler.Handle(request);
        return transactionsResult.IsSuccess ? Results.Ok(transactionsResult.Value) : Results.Problem(transactionsResult.Error.Message);
    });

    app.MapPost("/accounts", async (
        CreateAccountCommand request,
        ICommandHandler<CreateAccountCommand, AccountDto> commandHandler,
        IValidator<CreateAccountCommand> validator,
        ILogger<CreateAccountCommand> logger) =>
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogValidationFailure(validationResult, request);
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var accountResult = await commandHandler.Handle(request);
        return accountResult.IsSuccess ? Results.Created($"/accounts/{accountResult.Value.Id}", accountResult.Value) : Results.Problem(accountResult.Error.Message);
    });

    app.MapPost("/accounts/{accountId}/incomes", async (
        Guid accountId,
        AddIncomeCommand request,
        ICommandHandler<AddIncomeCommand, TransactionDto> commandHandler,
        IValidator<AddTransactionCommand> validator,
        ILogger<AddIncomeCommand> logger) =>
    {
        if (accountId != request.AccountId)
        {
            return Results.BadRequest("Account ID in the URL does not match the Account ID in the request body.");
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogValidationFailure(validationResult, request);
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var transactionResult = await commandHandler.Handle(request);
        return transactionResult.IsSuccess ? Results.Created($"/accounts/{accountId}/incomes/{transactionResult.Value.Id}", transactionResult.Value) : Results.Problem(transactionResult.Error.Message);
    });

    app.MapPost("/accounts/{accountId}/expenses", async (
        Guid accountId,
        AddExpenseCommand request,
        ICommandHandler<AddExpenseCommand, TransactionDto> commandHandler,
        IValidator<AddTransactionCommand> validator,
        ILogger<AddExpenseCommand> logger) =>
    {
        if (accountId != request.AccountId)
        {
            return Results.BadRequest("Account ID in the URL does not match the Account ID in the request body.");
        }

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            logger.LogValidationFailure(validationResult, request);
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var transactionResult = await commandHandler.Handle(request);
        return transactionResult.IsSuccess ? Results.Created($"/accounts/{accountId}/expenses/{transactionResult.Value.Id}", transactionResult.Value) : Results.Problem(transactionResult.Error.Message);
    });

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    await Log.CloseAndFlushAsync();
}
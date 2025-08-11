using Application.Common.Interfaces;
using FluentValidation.Results;

namespace Api.Extensions;

public static class ValidationLoggingExtensions
{
    public static void LogValidationFailure<TRequest>(this ILogger logger, ValidationResult validationResult, TRequest request)
        where TRequest : IBaseCommand
    {
        if (validationResult.IsValid)
        {
            return;
        }

        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        logger.LogWarning("Validation failed for {Operation}. Request: {@Request}. Errors: {@ValidationErrors}", request.GetType().Name, request, errors);
    }

    public static void LogValidationFailure<TRequest, TResponse>(this ILogger logger, ValidationResult validationResult, TRequest request)
        where TRequest : IQuery<TResponse>
    {
        if (validationResult.IsValid)
        {
            return;
        }

        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        logger.LogWarning("Validation failed for {Operation}. Request: {@Request}. Errors: {@ValidationErrors}", request.GetType().Name, request, errors);
    }
}

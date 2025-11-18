using Core.Application.Errors;
using Core.Application.Extensions;
using FastEndpoints;
using FluentResults;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Core.WebAPI.Extensions;

public static class FastEndpointsExtension
{
    private static Task<FastEndpoints.Void> ValidationErrorsAsync<TRequest, TResponse, TMediator>(this ResponseSender<TRequest, TResponse> responseSender, Result<TMediator> result, CancellationToken cancellationToken = default) where TRequest : notnull
    {
        responseSender.ValidationFailures.AddRange(result.GetValidationErrors().SelectMany(x => x.Value.Select(y => new ValidationFailure { PropertyName = x.Key, ErrorMessage = y })));

        return responseSender.ErrorsAsync(StatusCodes.Status400BadRequest, cancellationToken);
    }

    public static Task<FastEndpoints.Void> SendResponse<TRequest, TResponse, TMediator>(this ResponseSender<TRequest, TResponse> responseSender, Result<TMediator> result, Func<TMediator, TResponse> mapper, CancellationToken cancellationToken = default) where TRequest : notnull
    {
        return result switch
        {
            _ when result.HasError<NotFoundError>() => responseSender.ErrorsAsync(StatusCodes.Status404NotFound, cancellationToken),
            _ when result.HasError<SaveError>() => responseSender.ErrorsAsync(StatusCodes.Status500InternalServerError, cancellationToken),
            _ when result.HasError<ValidationError>() => responseSender.ValidationErrorsAsync(result, cancellationToken),

            _ when result.IsFailed => responseSender.ErrorsAsync(StatusCodes.Status500InternalServerError, cancellationToken),

            _ when result.Value is null => responseSender.NoContentAsync(cancellationToken),
            _ => responseSender.OkAsync(mapper(result.Value), cancellationToken),
        };
    }
}
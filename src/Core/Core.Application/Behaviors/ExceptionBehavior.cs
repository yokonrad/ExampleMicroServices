using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Behaviors;

public class ExceptionBehavior<TRequest, TResponse>(ILogger<ExceptionBehavior<TRequest, TResponse?>> logger) : IPipelineBehavior<TRequest, TResponse?> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse?> Handle(TRequest request, RequestHandlerDelegate<TResponse?> next, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("[{ClassName}] Handling request: {RequestName}", GetType().Name, typeof(TRequest).Name);

            var response = await next(cancellationToken);

            logger.LogInformation("[{ClassName}] Handled request: {RequestName} with response: {ResponseName}", GetType().Name, typeof(TRequest).Name, typeof(TResponse).Name);

            return response;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "[{ClassName}] Handled request: {RequestName} with response exception: {ExceptionMessage}", GetType().Name, typeof(TRequest).Name, exception.Message);

            return default;
        }
    }
}
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("[{ClassName}] Handling request: {RequestName}", GetType().Name, typeof(TRequest).Name);

        var response = await next(cancellationToken);

        logger.LogInformation("[{ClassName}] Handled request: {RequestName} with response: {ResponseName}", GetType().Name, typeof(TRequest).Name, typeof(TResponse).Name);

        return response;
    }
}
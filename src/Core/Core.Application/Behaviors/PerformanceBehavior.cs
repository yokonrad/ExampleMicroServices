using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Core.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("[{ClassName}] Handling request: {RequestName}", GetType().Name, typeof(TRequest).Name);

        var stopwatch = new Stopwatch();

        stopwatch.Start();

        var response = await next(cancellationToken);

        stopwatch.Stop();

        logger.LogInformation("[{ClassName}] Handled request: {RequestName} with response time: {ElapsedMilliseconds} ms", GetType().Name, typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
using FluentResults;
using MediatR;

namespace Core.WebAPI.Tests.Examples;

public record ExampleRequest() : IRequest<Result<string>>
{
    public required string Example { get; init; }
}

public class ExampleRequestHandler() : IRequestHandler<ExampleRequest, Result<string>>
{
    public Task<Result<string>> Handle(ExampleRequest exampleRequest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(exampleRequest.Example)) throw new NullReferenceException();

        return Task.FromResult(Result.Ok(exampleRequest.Example));
    }
}
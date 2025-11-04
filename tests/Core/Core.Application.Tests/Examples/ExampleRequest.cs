using FluentValidation;
using MediatR;

namespace Core.Application.Tests.Examples;

public record ExampleRequest() : IRequest<bool>
{
    public required string Example { get; init; }
}

public class ExampleRequestValidator : AbstractValidator<ExampleRequest>
{
    public ExampleRequestValidator()
    {
        RuleFor(x => x.Example).NotEmpty();
    }
}

public class ExampleRequestHandler() : IRequestHandler<ExampleRequest, bool>
{
    public Task<bool> Handle(ExampleRequest exampleRequest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(exampleRequest.Example)) throw new NullReferenceException();

        return Task.FromResult(true);
    }
}
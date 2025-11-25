using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;

namespace Core.WebAPI.Tests.Examples;

public record ExampleEndpointRequest()
{
    public required string Example { get; init; }
}

public record ExampleEndpointResponse()
{
    public required string Example { get; init; }
}

public class ExampleEndpointMapper : ResponseMapper<ExampleEndpointResponse, string>
{
    public override ExampleEndpointResponse FromEntity(string value)
    {
        return new()
        {
            Example = value,
        };
    }
}

public class ExampleEndpoint(IMediator mediator) : Endpoint<ExampleEndpointRequest, ExampleEndpointResponse, ExampleEndpointMapper>
{
    public override void Configure()
    {
        Get("example");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExampleEndpointRequest exampleEndpointRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new ExampleRequest
        {
            Example = exampleEndpointRequest.Example,
        }, ct), Map.FromEntity, ct);
    }
}
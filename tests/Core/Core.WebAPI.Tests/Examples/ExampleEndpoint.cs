using FastEndpoints;

namespace Core.WebAPI.Tests.Examples;

public record ExampleRequest()
{
    public required string Example { get; init; }
}

public record ExampleResponse()
{
    public required string Example { get; init; }
}

public class ExampleEndpoint : Endpoint<ExampleRequest, ExampleResponse>
{
    public override void Configure()
    {
        Get("example");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExampleRequest exampleRequest, CancellationToken ct)
    {
        await Send.OkAsync(new ExampleResponse { Example = exampleRequest.Example }, ct);
    }
}
using FastEndpoints;
using System.Net.Mime;

namespace Comments.WebAPI.Endpoints;

public record GetCommentsResponse()
{
    public required string Example { get; init; }
}

public class GetEndpoint() : EndpointWithoutRequest<IEnumerable<GetCommentsResponse>>
{
    public override void Configure()
    {
        Get("comments");
        Description(x =>
        {
            x.Produces<IEnumerable<GetCommentsResponse>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.ResponseAsync(Array.Empty<GetCommentsResponse>().AsEnumerable(), StatusCodes.Status200OK, ct);
    }
}
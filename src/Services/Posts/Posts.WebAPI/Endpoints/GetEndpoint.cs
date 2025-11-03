using FastEndpoints;
using System.Net.Mime;

namespace Posts.WebAPI.Endpoints;

public record GetPostsResponse()
{
    public required string Example { get; init; }
}

public class GetEndpoint() : EndpointWithoutRequest<IEnumerable<GetPostsResponse>>
{
    public override void Configure()
    {
        Get("posts");
        Description(x =>
        {
            x.Produces<IEnumerable<GetPostsResponse>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.ResponseAsync(Array.Empty<GetPostsResponse>().AsEnumerable(), StatusCodes.Status200OK, ct);
    }
}
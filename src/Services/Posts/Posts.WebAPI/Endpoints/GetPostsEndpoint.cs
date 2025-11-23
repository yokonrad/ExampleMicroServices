using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using Posts.Application.Dtos;
using Posts.Application.Queries;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Posts.WebAPI.Endpoints;

public record GetPostsResponse() : PostDto;

public class GetPostsMapper : ResponseMapper<IEnumerable<GetPostsResponse>, IEnumerable<PostDto>>
{
    public override IEnumerable<GetPostsResponse> FromEntity(IEnumerable<PostDto> postDtos)
    {
        return postDtos.Select(postDto => new GetPostsResponse
        {
            Guid = postDto.Guid,
            Title = postDto.Title,
            Text = postDto.Text,
            Visible = postDto.Visible,
        });
    }
}

public class GetPostsEndpoint(IMediator mediator) : EndpointWithoutRequest<IEnumerable<GetPostsResponse>, GetPostsMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Get("posts");
        Description(x =>
        {
            x.Produces<IEnumerable<GetPostsResponse>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new GetPostsQuery(), ct), Map.FromEntity, ct);
    }
}
using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using Posts.Application.Dtos;
using Posts.Application.Queries;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Posts.WebAPI.Endpoints;

public record GetPostByGuidRequest()
{
    [RouteParam]
    public required Guid Guid { get; init; }
}

public record GetPostByGuidResponse() : PostDto;

public class GetPostByGuidMapper : ResponseMapper<GetPostByGuidResponse, PostDto>
{
    public override GetPostByGuidResponse FromEntity(PostDto postDto)
    {
        return new()
        {
            Guid = postDto.Guid,
            Title = postDto.Title,
            Text = postDto.Text,
            Visible = postDto.Visible,
        };
    }
}

public class GetPostByGuidEndpoint(IMediator mediator) : Endpoint<GetPostByGuidRequest, GetPostByGuidResponse, GetPostByGuidMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Get("posts/{guid:guid}");
        Description(x =>
        {
            x.Accepts<GetPostByGuidRequest>();
            x.Produces<GetPostByGuidResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetPostByGuidRequest getPostByGuidRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new GetPostByGuidQuery
        {
            Guid = getPostByGuidRequest.Guid,
        }, ct), Map.FromEntity, ct);
    }
}
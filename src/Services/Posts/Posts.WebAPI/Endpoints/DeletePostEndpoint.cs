using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using Posts.Application.Commands;
using Posts.Application.Dtos;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Posts.WebAPI.Endpoints;

public record DeletePostRequest()
{
    [RouteParam]
    public required Guid Guid { get; init; }
}

public record DeletePostResponse() : PostDto;

public class DeletePostMapper : ResponseMapper<DeletePostResponse, PostDto>
{
    public override DeletePostResponse FromEntity(PostDto postDto)
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

public class DeletePostEndpoint(IMediator mediator) : Endpoint<DeletePostRequest, DeletePostResponse, DeletePostMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Delete("posts/{guid:guid}");
        Description(x =>
        {
            x.Accepts<DeletePostRequest>();
            x.Produces<DeletePostResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeletePostRequest deletePostRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new DeletePostCommand
        {
            Guid = deletePostRequest.Guid,
        }, ct), Map.FromEntity, ct);
    }
}
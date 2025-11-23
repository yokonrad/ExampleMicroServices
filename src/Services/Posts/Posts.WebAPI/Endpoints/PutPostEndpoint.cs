using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using Posts.Application.Commands;
using Posts.Application.Dtos;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Posts.WebAPI.Endpoints;

public record PutPostRequest()
{
    [RouteParam]
    public required Guid Guid { get; init; }

    [FormField]
    public required string Title { get; init; }

    [FormField]
    public required string Text { get; init; }

    [FormField]
    public required bool Visible { get; init; }
}

public record PutPostResponse() : PostDto;

public class PutPostMapper : ResponseMapper<PutPostResponse, PostDto>
{
    public override PutPostResponse FromEntity(PostDto postDto)
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

public class PutPostEndpoint(IMediator mediator) : Endpoint<PutPostRequest, PutPostResponse, PutPostMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Put("posts/{guid:guid}");
        Description(x =>
        {
            x.Accepts<PutPostRequest>(MediaTypeNames.Multipart.FormData);
            x.Produces<PutPostResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(PutPostRequest putPostRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new UpdatePostCommand
        {
            Guid = putPostRequest.Guid,
            Title = putPostRequest.Title,
            Text = putPostRequest.Text,
            Visible = putPostRequest.Visible,
        }, ct), Map.FromEntity, ct);
    }
}
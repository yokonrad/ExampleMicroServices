using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using Posts.Application.Commands;
using Posts.Application.Dtos;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Posts.WebAPI.Endpoints;

public record PatchPostRequest()
{
    [RouteParam]
    public required Guid Guid { get; init; }

    [FormField]
    public string? Title { get; init; }

    [FormField]
    public string? Text { get; init; }

    [FormField]
    public bool? Visible { get; init; }
}

public record PatchPostResponse() : PostDto;

public class PatchPostMapper : ResponseMapper<PatchPostResponse, PostDto>
{
    public override PatchPostResponse FromEntity(PostDto postDto)
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

public class PatchPostEndpoint(IMediator mediator) : Endpoint<PatchPostRequest, PatchPostResponse, PatchPostMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Patch("posts/{guid:guid}");
        Description(x =>
        {
            x.Accepts<PatchPostRequest>(MediaTypeNames.Multipart.FormData);
            x.Produces<PatchPostResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatchPostRequest patchPostRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new UpdatePartialPostCommand
        {
            Guid = patchPostRequest.Guid,
            Title = patchPostRequest.Title,
            Text = patchPostRequest.Text,
            Visible = patchPostRequest.Visible,
        }, ct), Map.FromEntity, ct);
    }
}
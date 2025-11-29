using Comments.Core.Commands;
using Comments.Core.Dtos;
using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Comments.WebAPI.Endpoints;

public record PatchCommentRequest()
{
    [RouteParam]
    public required Guid Guid { get; init; }

    [FormField]
    public required Guid PostGuid { get; init; }

    [FormField]
    public string? Text { get; init; }

    [FormField]
    public bool? Visible { get; init; }
}

public record PatchCommentResponse() : CommentDto;

public class PatchCommentMapper : ResponseMapper<PatchCommentResponse, CommentDto>
{
    public override PatchCommentResponse FromEntity(CommentDto commentDto)
    {
        return new()
        {
            Guid = commentDto.Guid,
            PostGuid = commentDto.PostGuid,
            Text = commentDto.Text,
            Visible = commentDto.Visible,
        };
    }
}

public class PatchCommentEndpoint(IMediator mediator) : Endpoint<PatchCommentRequest, PatchCommentResponse, PatchCommentMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Patch("comments/{guid:guid}");
        Description(x =>
        {
            x.Accepts<PatchCommentRequest>(MediaTypeNames.Multipart.FormData);
            x.Produces<PatchCommentResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatchCommentRequest patchCommentRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new UpdatePartialCommentCommand
        {
            Guid = patchCommentRequest.Guid,
            PostGuid = patchCommentRequest.PostGuid,
            Text = patchCommentRequest.Text,
            Visible = patchCommentRequest.Visible,
        }, ct), Map.FromEntity, ct);
    }
}
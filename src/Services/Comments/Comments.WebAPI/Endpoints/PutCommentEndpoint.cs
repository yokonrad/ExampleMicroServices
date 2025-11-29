using Comments.Core.Commands;
using Comments.Core.Dtos;
using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Comments.WebAPI.Endpoints;

public record PutCommentRequest()
{
    [RouteParam]
    public required Guid Guid { get; init; }

    [FormField]
    public required Guid PostGuid { get; init; }

    [FormField]
    public required string Text { get; init; }

    [FormField]
    public required bool Visible { get; init; }
}

public record PutCommentResponse() : CommentDto;

public class PutCommentMapper : ResponseMapper<PutCommentResponse, CommentDto>
{
    public override PutCommentResponse FromEntity(CommentDto commentDto)
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

public class PutCommentEndpoint(IMediator mediator) : Endpoint<PutCommentRequest, PutCommentResponse, PutCommentMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Put("comments/{guid:guid}");
        Description(x =>
        {
            x.Accepts<PutCommentRequest>(MediaTypeNames.Multipart.FormData);
            x.Produces<PutCommentResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(PutCommentRequest putCommentRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new UpdateCommentCommand
        {
            Guid = putCommentRequest.Guid,
            PostGuid = putCommentRequest.PostGuid,
            Text = putCommentRequest.Text,
            Visible = putCommentRequest.Visible,
        }, ct), Map.FromEntity, ct);
    }
}
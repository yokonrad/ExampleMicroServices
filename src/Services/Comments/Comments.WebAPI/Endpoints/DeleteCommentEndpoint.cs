using Comments.Core.Commands;
using Comments.Core.Dtos;
using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Comments.WebAPI.Endpoints;

public record DeleteCommentRequest()
{
    [RouteParam]
    public required Guid Guid { get; init; }
}

public record DeleteCommentResponse() : CommentDto;

public class DeleteCommentMapper : ResponseMapper<DeleteCommentResponse, CommentDto>
{
    public override DeleteCommentResponse FromEntity(CommentDto commentDto)
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

public class DeleteCommentEndpoint(IMediator mediator) : Endpoint<DeleteCommentRequest, DeleteCommentResponse, DeleteCommentMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Delete("comments/{guid:guid}");
        Description(x =>
        {
            x.Accepts<DeleteCommentRequest>();
            x.Produces<DeleteCommentResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteCommentRequest deleteCommentRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new DeleteCommentCommand
        {
            Guid = deleteCommentRequest.Guid,
        }, ct), Map.FromEntity, ct);
    }
}
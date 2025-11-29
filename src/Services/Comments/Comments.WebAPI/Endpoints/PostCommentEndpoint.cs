using Comments.Core.Commands;
using Comments.Core.Dtos;
using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Comments.WebAPI.Endpoints;

public record PostCommentRequest()
{
    [FormField]
    public required Guid PostGuid { get; init; }

    [FormField]
    public required string Text { get; init; }

    [FormField]
    public required bool Visible { get; init; }
}

public record PostCommentResponse() : CommentDto;

public class PostCommentMapper : ResponseMapper<PostCommentResponse, CommentDto>
{
    public override PostCommentResponse FromEntity(CommentDto commentDto)
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

public class PostCommentEndpoint(IMediator mediator) : Endpoint<PostCommentRequest, PostCommentResponse, PostCommentMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Post("comments");
        Description(x =>
        {
            x.Accepts<PostCommentRequest>(MediaTypeNames.Multipart.FormData);
            x.Produces<PostCommentResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(PostCommentRequest patchCommentRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new CreateCommentCommand
        {
            PostGuid = patchCommentRequest.PostGuid,
            Text = patchCommentRequest.Text,
            Visible = patchCommentRequest.Visible,
        }, ct), Map.FromEntity, ct);
    }
}
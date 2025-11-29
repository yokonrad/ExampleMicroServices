using Comments.Core.Dtos;
using Comments.Core.Queries;
using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Comments.WebAPI.Endpoints;

public record GetCommentsByPostGuidRequest()
{
    [RouteParam]
    public required Guid PostGuid { get; init; }
}

public record GetCommentsByPostGuidResponse() : CommentDto;

public class GetCommentsByPostGuidMapper : ResponseMapper<IEnumerable<GetCommentsByPostGuidResponse>, IEnumerable<CommentDto>>
{
    public override IEnumerable<GetCommentsByPostGuidResponse> FromEntity(IEnumerable<CommentDto> commentDtos)
    {
        return commentDtos.Select(commentDto => new GetCommentsByPostGuidResponse
        {
            Guid = commentDto.Guid,
            PostGuid = commentDto.PostGuid,
            Text = commentDto.Text,
            Visible = commentDto.Visible,
        });
    }
}

public class GetCommentsByPostGuidEndpoint(IMediator mediator) : Endpoint<GetCommentsByPostGuidRequest, IEnumerable<GetCommentsByPostGuidResponse>, GetCommentsByPostGuidMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Get("comments/post/{postGuid:guid}");
        Description(x =>
        {
            x.Accepts<GetCommentsByPostGuidRequest>();
            x.Produces<GetCommentsByPostGuidResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetCommentsByPostGuidRequest getCommentsByPostGuidRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new GetCommentsByPostGuidQuery
        {
            PostGuid = getCommentsByPostGuidRequest.PostGuid,
        }, ct), Map.FromEntity, ct);
    }
}
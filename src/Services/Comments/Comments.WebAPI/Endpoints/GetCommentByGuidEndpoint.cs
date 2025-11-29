using Comments.Core.Dtos;
using Comments.Core.Queries;
using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Comments.WebAPI.Endpoints;

public record GetCommentByGuidRequest()
{
    [RouteParam]
    public required Guid Guid { get; init; }
}

public record GetCommentByGuidResponse() : CommentDto;

public class GetCommentByGuidMapper : ResponseMapper<GetCommentByGuidResponse, CommentDto>
{
    public override GetCommentByGuidResponse FromEntity(CommentDto postDto)
    {
        return new()
        {
            Guid = postDto.Guid,
            PostGuid = postDto.PostGuid,
            Text = postDto.Text,
            Visible = postDto.Visible,
        };
    }
}

public class GetCommentByGuidEndpoint(IMediator mediator) : Endpoint<GetCommentByGuidRequest, GetCommentByGuidResponse, GetCommentByGuidMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Get("comments/{guid:guid}");
        Description(x =>
        {
            x.Accepts<GetCommentByGuidRequest>();
            x.Produces<GetCommentByGuidResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetCommentByGuidRequest getCommentByGuidRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new GetCommentByGuidQuery
        {
            Guid = getCommentByGuidRequest.Guid,
        }, ct), Map.FromEntity, ct);
    }
}
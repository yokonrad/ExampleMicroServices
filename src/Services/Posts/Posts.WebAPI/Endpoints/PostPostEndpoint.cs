using Core.WebAPI.Extensions;
using FastEndpoints;
using MediatR;
using Posts.Application.Commands;
using Posts.Application.Dtos;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace Posts.WebAPI.Endpoints;

public record PostPostRequest()
{
    [FormField]
    public required string Title { get; init; }

    [FormField]
    public required string Text { get; init; }

    [FormField]
    public required bool Visible { get; init; }
}

public record PostPostResponse() : PostDto;

public class PostPostMapper : ResponseMapper<PostPostResponse, PostDto>
{
    public override PostPostResponse FromEntity(PostDto postDto)
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

public class PostPostEndpoint(IMediator mediator) : Endpoint<PostPostRequest, PostPostResponse, PostPostMapper>
{
    [ExcludeFromCodeCoverage]
    public override void Configure()
    {
        Post("posts");
        Description(x =>
        {
            x.Accepts<PostPostRequest>(MediaTypeNames.Multipart.FormData);
            x.Produces<PostPostResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
            x.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);
            x.ClearDefaultProduces();
        });
        AllowAnonymous();
    }

    public override async Task HandleAsync(PostPostRequest postPostRequest, CancellationToken ct)
    {
        await Send.SendResponse(await mediator.Send(new CreatePostCommand
        {
            Title = postPostRequest.Title,
            Text = postPostRequest.Text,
            Visible = postPostRequest.Visible,
        }, ct), Map.FromEntity, ct);
    }
}
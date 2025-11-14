using AutoMapper;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentResults;
using FluentValidation;
using MediatR;
using Posts.Application.Dtos;
using Posts.Application.Notifications;
using Posts.Domain.Entities;
using Posts.Domain.Interfaces;

namespace Posts.Application.Commands;

public record CreatePostCommand() : IRequest<Result<PostDto>>
{
    public required string Title { get; init; }
    public required string Text { get; init; }
    public required bool Visible { get; init; }
}

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Text).NotEmpty().MinimumLength(3);
    }
}

public class CreatePostCommandHandler(IMapper mapper, IMediator mediator, IValidator<CreatePostCommand> validator, IPostRepository postRepository) : IRequestHandler<CreatePostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(CreatePostCommand createPostCommand, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(createPostCommand, cancellationToken);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var post = mapper.Map<CreatePostCommand, Post>(createPostCommand);

        if (!await postRepository.CreateAsync(post, cancellationToken)) return Result.Fail(new SaveError());

        var postDto = mapper.Map<Post, PostDto>(post);

        await mediator.Publish(new CreatedPostNotification(postDto), cancellationToken);

        return Result.Ok(postDto);
    }
}
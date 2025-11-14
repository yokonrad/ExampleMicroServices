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

public record UpdatePostCommand() : IRequest<Result<PostDto>>
{
    public required Guid Guid { get; init; }
    public required string Title { get; init; }
    public required string Text { get; init; }
    public required bool Visible { get; init; }
}

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.Guid).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Text).NotEmpty().MinimumLength(3);
    }
}

public class UpdatePostCommandHandler(IMapper mapper, IMediator mediator, IValidator<UpdatePostCommand> validator, IPostRepository postRepository) : IRequestHandler<UpdatePostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(UpdatePostCommand updatePostCommand, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(updatePostCommand, cancellationToken);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var post = await postRepository.GetByGuidAsync(updatePostCommand.Guid, cancellationToken);

        if (post is null) return Result.Fail(new NotFoundError());

        post.Title = updatePostCommand.Title;
        post.Text = updatePostCommand.Text;
        post.Visible = updatePostCommand.Visible;

        if (!await postRepository.UpdateAsync(post, cancellationToken)) return Result.Fail(new SaveError());

        var postDto = mapper.Map<Post, PostDto>(post);

        await mediator.Publish(new UpdatedPostNotification(postDto), cancellationToken);

        return Result.Ok(postDto);
    }
}
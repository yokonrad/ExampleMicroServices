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

public record UpdatePartialPostCommand() : IRequest<Result<PostDto>>
{
    public required Guid Guid { get; init; }
    public string? Title { get; init; }
    public string? Text { get; init; }
    public bool? Visible { get; init; }
}

public class UpdatePartialPostCommandValidator : AbstractValidator<UpdatePartialPostCommand>
{
    public UpdatePartialPostCommandValidator()
    {
        RuleFor(x => x.Guid).NotEmpty();
        RuleFor(x => x.Title).MinimumLength(3);
        RuleFor(x => x.Text).MinimumLength(3);
    }
}

public class UpdatePartialPostCommandHandler(IMapper mapper, IMediator mediator, IValidator<UpdatePartialPostCommand> validator, IPostRepository postRepository) : IRequestHandler<UpdatePartialPostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(UpdatePartialPostCommand updatePartialPostCommand, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(updatePartialPostCommand, cancellationToken);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var post = await postRepository.GetByGuidAsync(updatePartialPostCommand.Guid, cancellationToken);

        if (post is null) return Result.Fail(new NotFoundError());

        post.Title = updatePartialPostCommand.Title switch
        {
            null => post.Title,
            _ => updatePartialPostCommand.Title,
        };

        post.Text = updatePartialPostCommand.Text switch
        {
            null => post.Text,
            _ => updatePartialPostCommand.Text,
        };

        post.Visible = updatePartialPostCommand.Visible switch
        {
            null => post.Visible,
            _ => updatePartialPostCommand.Visible.Value,
        };

        if (!await postRepository.UpdateAsync(post, cancellationToken)) return Result.Fail(new SaveError());

        var postDto = mapper.Map<Post, PostDto>(post);

        await mediator.Publish(new UpdatedPartiallyPostNotification(postDto), cancellationToken);

        return Result.Ok(postDto);
    }
}
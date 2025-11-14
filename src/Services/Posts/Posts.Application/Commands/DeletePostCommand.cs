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

public record DeletePostCommand() : IRequest<Result<PostDto>>
{
    public required Guid Guid { get; init; }
}

public class DeletePostCommandValidator : AbstractValidator<DeletePostCommand>
{
    public DeletePostCommandValidator()
    {
        RuleFor(x => x.Guid).NotEmpty();
    }
}

public class DeletePostCommandHandler(IMapper mapper, IMediator mediator, IValidator<DeletePostCommand> validator, IPostRepository postRepository) : IRequestHandler<DeletePostCommand, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(DeletePostCommand deletePostCommand, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(deletePostCommand, cancellationToken);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var post = await postRepository.GetByGuidAsync(deletePostCommand.Guid, cancellationToken);

        if (post is null) return Result.Fail(new NotFoundError());

        if (!await postRepository.DeleteAsync(post, cancellationToken)) return Result.Fail(new SaveError());

        var postDto = mapper.Map<Post, PostDto>(post);

        await mediator.Publish(new DeletedPostNotification(postDto), cancellationToken);

        return Result.Ok(postDto);
    }
}
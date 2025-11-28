using AutoMapper;
using Comments.Core.Dtos;
using Comments.Core.Entities;
using Comments.Core.Interfaces;
using Comments.Core.Notifications;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Comments.Core.Commands;

public record DeleteCommentCommand() : IRequest<Result<CommentDto>>
{
    public required Guid Guid { get; init; }
}

public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.Guid).NotEmpty();
    }
}

public class DeleteCommentCommandHandler(IMapper mapper, IMediator mediator, IValidator<DeleteCommentCommand> validator, ICommentRepository commentRepository, IPostService postService) : IRequestHandler<DeleteCommentCommand, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(DeleteCommentCommand deleteCommentCommand, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(deleteCommentCommand, cancellationToken);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var comment = await commentRepository.GetByGuidAsync(deleteCommentCommand.Guid, cancellationToken);

        if (comment is null) return Result.Fail(new NotFoundError());

        var postDto = await postService.GetByGuidAsync(comment.PostGuid, cancellationToken);

        if (postDto is null) return Result.Fail(new ServiceError());

        if (!await commentRepository.DeleteAsync(comment, cancellationToken)) return Result.Fail(new SaveError());

        var commentDto = mapper.Map<Comment, CommentDto>(comment);

        await mediator.Publish(new DeletedCommentNotification(commentDto), cancellationToken);

        return Result.Ok(commentDto);
    }
}
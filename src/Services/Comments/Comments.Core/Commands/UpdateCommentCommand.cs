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

public record UpdateCommentCommand() : IRequest<Result<CommentDto>>
{
    public required Guid Guid { get; init; }
    public required Guid PostGuid { get; init; }
    public required string Text { get; init; }
    public required bool Visible { get; init; }
}

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.Guid).NotEmpty();
        RuleFor(x => x.PostGuid).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MinimumLength(3);
    }
}

public class UpdateCommentCommandHandler(IMapper mapper, IMediator mediator, IValidator<UpdateCommentCommand> validator, ICommentRepository commentRepository, IPostService postService) : IRequestHandler<UpdateCommentCommand, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(UpdateCommentCommand updateCommentCommand, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(updateCommentCommand, cancellationToken);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var comment = await commentRepository.GetByGuidAsync(updateCommentCommand.Guid, cancellationToken);

        if (comment is null) return Result.Fail(new NotFoundError());

        var postDto = await postService.GetByGuidAsync(updateCommentCommand.PostGuid, cancellationToken);

        if (postDto is null) return Result.Fail(new ServiceError());

        comment.PostGuid = updateCommentCommand.PostGuid;
        comment.Text = updateCommentCommand.Text;
        comment.Visible = updateCommentCommand.Visible;

        if (!await commentRepository.UpdateAsync(comment, cancellationToken)) return Result.Fail(new SaveError());

        var commentDto = mapper.Map<Comment, CommentDto>(comment);

        await mediator.Publish(new UpdatedCommentNotification(commentDto), cancellationToken);

        return Result.Ok(commentDto);
    }
}
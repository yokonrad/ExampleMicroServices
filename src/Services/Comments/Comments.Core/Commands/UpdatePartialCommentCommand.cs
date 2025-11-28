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

public record UpdatePartialCommentCommand() : IRequest<Result<CommentDto>>
{
    public required Guid Guid { get; init; }
    public required Guid PostGuid { get; init; }
    public string? Text { get; init; }
    public bool? Visible { get; init; }
}

public class UpdatePartialCommentCommandValidator : AbstractValidator<UpdatePartialCommentCommand>
{
    public UpdatePartialCommentCommandValidator()
    {
        RuleFor(x => x.Guid).NotEmpty();
        RuleFor(x => x.PostGuid).NotEmpty();
        RuleFor(x => x.Text).MinimumLength(3);
    }
}

public class UpdatePartialCommentCommandHandler(IMapper mapper, IMediator mediator, IValidator<UpdatePartialCommentCommand> validator, ICommentRepository commentRepository, IPostService postService) : IRequestHandler<UpdatePartialCommentCommand, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(UpdatePartialCommentCommand updatePartialCommentCommand, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(updatePartialCommentCommand, cancellationToken);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var comment = await commentRepository.GetByGuidAsync(updatePartialCommentCommand.Guid, cancellationToken);

        if (comment is null) return Result.Fail(new NotFoundError());

        var postDto = await postService.GetByGuidAsync(updatePartialCommentCommand.PostGuid, cancellationToken);

        if (postDto is null) return Result.Fail(new ServiceError());

        comment.Text = updatePartialCommentCommand.Text switch
        {
            null => comment.Text,
            _ => updatePartialCommentCommand.Text,
        };

        comment.Visible = updatePartialCommentCommand.Visible switch
        {
            null => comment.Visible,
            _ => updatePartialCommentCommand.Visible.Value,
        };

        if (!await commentRepository.UpdateAsync(comment, cancellationToken)) return Result.Fail(new SaveError());

        var commentDto = mapper.Map<Comment, CommentDto>(comment);

        await mediator.Publish(new UpdatedPartiallyCommentNotification(commentDto), cancellationToken);

        return Result.Ok(commentDto);
    }
}
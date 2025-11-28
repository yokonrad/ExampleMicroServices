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

public record CreateCommentCommand() : IRequest<Result<CommentDto>>
{
    public required Guid PostGuid { get; init; }
    public required string Text { get; init; }
    public required bool Visible { get; init; }
}

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.PostGuid).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MinimumLength(3);
    }
}

public class CreateCommentCommandHandler(IMapper mapper, IMediator mediator, IValidator<CreateCommentCommand> validator, IPostService postService, ICommentRepository commentRepository) : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(CreateCommentCommand createCommentCommand, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(createCommentCommand, cancellationToken);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var postDto = await postService.GetByGuidAsync(createCommentCommand.PostGuid, cancellationToken);

        if (postDto is null) return Result.Fail(new ServiceError());

        var comment = mapper.Map<CreateCommentCommand, Comment>(createCommentCommand);

        if (!await commentRepository.CreateAsync(comment, cancellationToken)) return Result.Fail(new SaveError());

        var commentDto = mapper.Map<Comment, CommentDto>(comment);

        await mediator.Publish(new CreatedCommentNotification(commentDto), cancellationToken);

        return Result.Ok(commentDto);
    }
}
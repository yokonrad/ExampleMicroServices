using AutoMapper;
using Comments.Core.Dtos;
using Comments.Core.Entities;
using Comments.Core.Interfaces;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Comments.Core.Queries;

public record GetCommentByGuidQuery() : IRequest<Result<CommentDto>>
{
    public required Guid Guid { get; init; }
}

public class GetCommentByGuidQueryValidator : AbstractValidator<GetCommentByGuidQuery>
{
    public GetCommentByGuidQueryValidator()
    {
        RuleFor(x => x.Guid).NotEmpty();
    }
}

public class GetCommentByGuidQueryHandler(IMapper mapper, IValidator<GetCommentByGuidQuery> validator, ICommentRepository commentRepository, IPostService postService) : IRequestHandler<GetCommentByGuidQuery, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(GetCommentByGuidQuery getCommentByGuidQuery, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(getCommentByGuidQuery);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var comment = await commentRepository.GetByGuidAsync(getCommentByGuidQuery.Guid, cancellationToken);

        if (comment is null) return Result.Fail(new NotFoundError());

        var postDto = await postService.GetByGuidAsync(comment.PostGuid, cancellationToken);

        if (postDto is null) return Result.Fail(new ServiceError());

        return Result.Ok(mapper.Map<Comment, CommentDto>(comment));
    }
}
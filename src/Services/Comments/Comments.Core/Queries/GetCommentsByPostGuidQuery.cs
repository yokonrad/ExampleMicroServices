using AutoMapper;
using Comments.Core.Dtos;
using Comments.Core.Interfaces;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Comments.Core.Queries;

public class GetCommentsByPostGuidQuery() : IRequest<Result<IEnumerable<CommentDto>>>
{
    public Guid PostGuid { get; init; }
}

public class GetCommentsByPostGuidQueryValidator : AbstractValidator<GetCommentsByPostGuidQuery>
{
    public GetCommentsByPostGuidQueryValidator()
    {
        RuleFor(x => x.PostGuid).NotEmpty();
    }
}

public class GetCommentsByPostGuidQueryHandler(IMapper mapper, IValidator<GetCommentsByPostGuidQuery> validator, IPostService postService, ICommentRepository commentRepository) : IRequestHandler<GetCommentsByPostGuidQuery, Result<IEnumerable<CommentDto>>>
{
    public async Task<Result<IEnumerable<CommentDto>>> Handle(GetCommentsByPostGuidQuery getCommentsByPostGuidQuery, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(getCommentsByPostGuidQuery);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var postDto = await postService.GetByGuidAsync(getCommentsByPostGuidQuery.PostGuid, cancellationToken);

        if (postDto is null) return Result.Fail(new ServiceError());

        var comments = await commentRepository.GetByPostGuidAsync(postDto.Guid, cancellationToken);

        return Result.Ok(mapper.Map<IEnumerable<CommentDto>>(comments));
    }
}
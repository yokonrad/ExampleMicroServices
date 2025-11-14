using AutoMapper;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentResults;
using FluentValidation;
using MediatR;
using Posts.Application.Dtos;
using Posts.Domain.Entities;
using Posts.Domain.Interfaces;

namespace Posts.Application.Queries;

public record GetPostByGuidQuery() : IRequest<Result<PostDto>>
{
    public required Guid Guid { get; init; }
}

public class GetPostByGuidQueryValidator : AbstractValidator<GetPostByGuidQuery>
{
    public GetPostByGuidQueryValidator()
    {
        RuleFor(x => x.Guid).NotEmpty();
    }
}

public class GetPostByGuidQueryHandler(IMapper mapper, IValidator<GetPostByGuidQuery> validator, IPostRepository postRepository) : IRequestHandler<GetPostByGuidQuery, Result<PostDto>>
{
    public async Task<Result<PostDto>> Handle(GetPostByGuidQuery getPostByGuidQuery, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(getPostByGuidQuery);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var post = await postRepository.GetByGuidAsync(getPostByGuidQuery.Guid, cancellationToken);

        if (post is null) return Result.Fail(new NotFoundError());

        return Result.Ok(mapper.Map<Post, PostDto>(post));
    }
}
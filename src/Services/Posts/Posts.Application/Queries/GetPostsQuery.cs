using AutoMapper;
using Core.Application.Extensions;
using FluentResults;
using FluentValidation;
using MediatR;
using Posts.Application.Dtos;
using Posts.Domain.Interfaces;

namespace Posts.Application.Queries;

public record GetPostsQuery() : IRequest<Result<IEnumerable<PostDto>>>;

public class GetPostsQueryValidator : AbstractValidator<GetPostsQuery>;

public class GetPostsQueryHandler(IMapper mapper, IValidator<GetPostsQuery> validator, IPostRepository postRepository) : IRequestHandler<GetPostsQuery, Result<IEnumerable<PostDto>>>
{
    public async Task<Result<IEnumerable<PostDto>>> Handle(GetPostsQuery getPostsQuery, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(getPostsQuery);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var posts = await postRepository.GetAsync(cancellationToken);

        return Result.Ok(mapper.Map<IEnumerable<PostDto>>(posts));
    }
}
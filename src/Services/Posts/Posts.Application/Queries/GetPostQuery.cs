using AutoMapper;
using Core.Application.Extensions;
using FluentResults;
using FluentValidation;
using MediatR;
using Posts.Application.Dtos;
using Posts.Domain.Interfaces;

namespace Posts.Application.Queries;

public record GetPostQuery() : IRequest<Result<IEnumerable<PostDto>>>;

public class GetPostQueryValidator : AbstractValidator<GetPostQuery>;

public class GetPostQueryHandler(IMapper mapper, IValidator<GetPostQuery> validator, IPostRepository postRepository) : IRequestHandler<GetPostQuery, Result<IEnumerable<PostDto>>>
{
    public async Task<Result<IEnumerable<PostDto>>> Handle(GetPostQuery getPostQuery, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(getPostQuery);

        if (!validationResult.IsValid) return Result.Fail(validationResult.GetValidationErrors());

        var posts = await postRepository.GetAsync(cancellationToken);

        return Result.Ok(mapper.Map<IEnumerable<PostDto>>(posts));
    }
}
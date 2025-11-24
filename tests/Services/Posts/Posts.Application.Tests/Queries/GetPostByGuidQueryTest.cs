using AutoBogus;
using AutoMapper;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Posts.Application.Dtos;
using Posts.Application.Extensions;
using Posts.Application.Queries;
using Posts.Domain.Entities;
using Posts.Domain.Interfaces;

namespace Posts.Application.Tests.Queries;

public class GetPostByGuidQueryTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IValidator<GetPostByGuidQuery> validator;
    private Mock<IPostRepository> mockPostRepository;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPostsApplicationSupport();
        serviceCollection.AddLogging(x => x.AddProvider(NullLoggerProvider.Instance));

        serviceProvider = serviceCollection.BuildServiceProvider();
        mapper = serviceProvider.GetRequiredService<IMapper>();
        validator = serviceProvider.GetRequiredService<IValidator<GetPostByGuidQuery>>();
        mockPostRepository = new Mock<IPostRepository>();
    }

    [TearDown]
    public async Task TearDown()
    {
        await serviceProvider.DisposeAsync();
    }

    [Test]
    public async Task Should_Be_Invalid_When_ValidationError()
    {
        // Arrange
        var fakerGetPostByGuidQuery = new AutoFaker<GetPostByGuidQuery>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>());

        var getPostByGuidQuery = fakerGetPostByGuidQuery.Generate();

        var validationResult = validator.Validate(getPostByGuidQuery);
        var validationResultErrors = validationResult.GetValidationErrors();

        var getPostByGuidQueryHandler = new GetPostByGuidQueryHandler(mapper, validator, mockPostRepository.Object);

        // Act
        var act = await getPostByGuidQueryHandler.Handle(getPostByGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_NotFoundError()
    {
        // Arrange
        var fakerGetPostByGuidQuery = new AutoFaker<GetPostByGuidQuery>();
        var getPostByGuidQuery = fakerGetPostByGuidQuery.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(getPostByGuidQuery.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Post>());

        var getPostByGuidQueryHandler = new GetPostByGuidQueryHandler(mapper, validator, mockPostRepository.Object);

        // Act
        var act = await getPostByGuidQueryHandler.Handle(getPostByGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<NotFoundError>(out var actErrors).Should().BeTrue();
        actErrors.Should().BeEquivalentTo([new NotFoundError()]);
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();
        var post = fakerPost.Generate();
        var postDto = mapper.Map<Post, PostDto>(post);

        var fakerGetPostByGuidQuery = new AutoFaker<GetPostByGuidQuery>()
            .RuleFor(x => x.Guid, _ => post.Guid);

        var getPostByGuidQuery = fakerGetPostByGuidQuery.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(getPostByGuidQuery.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);

        var getPostByGuidQueryHandler = new GetPostByGuidQueryHandler(mapper, validator, mockPostRepository.Object);

        // Act
        var act = await getPostByGuidQueryHandler.Handle(getPostByGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
    }
}
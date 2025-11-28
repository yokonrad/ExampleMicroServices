using AutoBogus;
using AutoMapper;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
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

public class GetPostsQueryTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private Mock<IValidator<GetPostsQuery>> mockValidator;
    private Mock<IPostRepository> mockPostRepository;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPostsApplicationSupport();
        serviceCollection.AddLogging(x => x.AddProvider(NullLoggerProvider.Instance));

        serviceProvider = serviceCollection.BuildServiceProvider();
        mapper = serviceProvider.GetRequiredService<IMapper>();
        mockValidator = new Mock<IValidator<GetPostsQuery>>();
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
        var fakerGetPostsQuery = new AutoFaker<GetPostsQuery>();
        var getPostsQuery = fakerGetPostsQuery.Generate();

        mockValidator.Setup(x => x.Validate(getPostsQuery)).Returns(new ValidationResult([new ValidationFailure("Property name", "Error message")]));

        var validationResult = mockValidator.Object.Validate(getPostsQuery);
        var validationResultErrors = validationResult.GetValidationErrors();

        var getPostsQueryHandler = new GetPostsQueryHandler(mapper, mockValidator.Object, mockPostRepository.Object);

        // Act
        var act = await getPostsQueryHandler.Handle(getPostsQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Valid_When_Empty()
    {
        // Arrange
        var posts = Array.Empty<Post>();
        var postDtos = mapper.Map<IEnumerable<PostDto>>(posts);

        var fakerGetPostsQuery = new AutoFaker<GetPostsQuery>();
        var getPostsQuery = fakerGetPostsQuery.Generate();

        mockPostRepository.Setup(x => x.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(posts);

        var getPostsQueryHandler = new GetPostsQueryHandler(mapper, mockValidator.Object, mockPostRepository.Object);

        // Act
        var act = await getPostsQueryHandler.Handle(getPostsQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeAssignableTo<IEnumerable<PostDto>>().And.BeEquivalentTo(postDtos).And.BeEmpty();
    }

    [Test]
    public async Task Should_Be_Valid_When_Not_Empty()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();
        var posts = fakerPost.Generate(100).ToArray();
        var postDtos = mapper.Map<IEnumerable<PostDto>>(posts);

        var fakerGetPostsQuery = new AutoFaker<GetPostsQuery>();
        var getPostsQuery = fakerGetPostsQuery.Generate();

        mockPostRepository.Setup(x => x.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(posts);

        var getPostsQueryHandler = new GetPostsQueryHandler(mapper, mockValidator.Object, mockPostRepository.Object);

        // Act
        var act = await getPostsQueryHandler.Handle(getPostsQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeAssignableTo<IEnumerable<PostDto>>().And.BeEquivalentTo(postDtos).And.NotBeEmpty();
    }
}
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
        var getPostByGuidQuery = new AutoFaker<GetPostByGuidQuery>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .Generate();

        var validationResult = validator.Validate(getPostByGuidQuery);
        var validationResultErrors = validationResult.GetValidationErrors();

        var getPostByGuidQueryHandler = new GetPostByGuidQueryHandler(mapper, validator, mockPostRepository.Object);

        // Act
        var act = await getPostByGuidQueryHandler.Handle(getPostByGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_NotFoundError()
    {
        // Arrange
        var getPostByGuidQuery = new AutoFaker<GetPostByGuidQuery>()
            .Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(getPostByGuidQuery.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Post>());

        var getPostByGuidQueryHandler = new GetPostByGuidQueryHandler(mapper, validator, mockPostRepository.Object);

        // Act
        var act = await getPostByGuidQueryHandler.Handle(getPostByGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<NotFoundError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new NotFoundError()]);
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var getPostByGuidQuery = new AutoFaker<GetPostByGuidQuery>()
            .Generate();

        var post = new AutoFaker<Post>()
            .RuleFor(x => x.Guid, _ => getPostByGuidQuery.Guid)
            .Generate();

        var postDto = mapper.Map<Post, PostDto>(post);

        mockPostRepository.Setup(x => x.GetByGuidAsync(getPostByGuidQuery.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);

        var getPostByGuidQueryHandler = new GetPostByGuidQueryHandler(mapper, validator, mockPostRepository.Object);

        // Act
        var act = await getPostByGuidQueryHandler.Handle(getPostByGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
    }
}
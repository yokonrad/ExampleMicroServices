using AutoBogus;
using AutoMapper;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Posts.Application.Commands;
using Posts.Application.Dtos;
using Posts.Application.Extensions;
using Posts.Domain.Entities;
using Posts.Domain.Interfaces;

namespace Posts.Application.Tests.Commands;

public class CreatePostCommandTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IMediator mediator;
    private IValidator<CreatePostCommand> validator;
    private Mock<IPostRepository> mockPostRepository;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPostsApplicationSupport();
        serviceCollection.AddLogging(x => x.AddProvider(NullLoggerProvider.Instance));

        serviceProvider = serviceCollection.BuildServiceProvider();
        mapper = serviceProvider.GetRequiredService<IMapper>();
        mediator = serviceProvider.GetRequiredService<IMediator>();
        validator = serviceProvider.GetRequiredService<IValidator<CreatePostCommand>>();
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
        var fakerCreatePostCommand = new AutoFaker<CreatePostCommand>()
            .RuleFor(x => x.Title, _ => It.IsAny<string>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>());

        var createPostCommand = fakerCreatePostCommand.Generate();

        var validationResult = validator.Validate(createPostCommand);
        var validationResultErrors = validationResult.GetValidationErrors();

        var createPostCommandHandler = new CreatePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await createPostCommandHandler.Handle(createPostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_SaveError()
    {
        // Arrange
        var fakerCreatePostCommand = new AutoFaker<CreatePostCommand>()
            .RuleFor(x => x.Title, x => x.Random.String2(3, 25))
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var createPostCommand = fakerCreatePostCommand.Generate();

        var post = mapper.Map<CreatePostCommand, Post>(createPostCommand);

        mockPostRepository.Setup(x => x.CreateAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var createPostCommandHandler = new CreatePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await createPostCommandHandler.Handle(createPostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<SaveError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new SaveError()]);
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var fakerCreatePostCommand = new AutoFaker<CreatePostCommand>()
            .RuleFor(x => x.Title, x => x.Random.String2(3, 25))
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var createPostCommand = fakerCreatePostCommand.Generate();

        var post = mapper.Map<CreatePostCommand, Post>(createPostCommand);
        var postDto = mapper.Map<Post, PostDto>(post);

        mockPostRepository.Setup(x => x.CreateAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var createPostCommandHandler = new CreatePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await createPostCommandHandler.Handle(createPostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
    }
}
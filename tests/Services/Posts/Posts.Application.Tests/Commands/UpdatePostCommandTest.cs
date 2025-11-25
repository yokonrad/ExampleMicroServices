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

public class UpdatePostCommandTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IMediator mediator;
    private IValidator<UpdatePostCommand> validator;
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
        validator = serviceProvider.GetRequiredService<IValidator<UpdatePostCommand>>();
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
        var fakerUpdatePostCommand = new AutoFaker<UpdatePostCommand>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.Title, _ => It.IsAny<string>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>());

        var updatePostCommand = fakerUpdatePostCommand.Generate();

        var validationResult = validator.Validate(updatePostCommand);
        var validationResultErrors = validationResult.GetValidationErrors();

        var updatePostCommandHandler = new UpdatePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePostCommandHandler.Handle(updatePostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_NotFoundError()
    {
        // Arrange
        var fakerUpdatePostCommand = new AutoFaker<UpdatePostCommand>();
        var updatePostCommand = fakerUpdatePostCommand.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(updatePostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Post>());

        var updatePostCommandHandler = new UpdatePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePostCommandHandler.Handle(updatePostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<NotFoundError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new NotFoundError()]);
    }

    [Test]
    public async Task Should_Be_Invalid_When_SaveError()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Title, x => x.Random.String2(3, 25))
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var post = fakerPost.Generate();

        var fakerUpdatePostCommand = new AutoFaker<UpdatePostCommand>()
            .RuleFor(x => x.Guid, _ => post.Guid)
            .RuleFor(x => x.Title, _ => post.Title)
            .RuleFor(x => x.Text, _ => post.Text)
            .RuleFor(x => x.Visible, _ => post.Visible);

        var updatePostCommand = fakerUpdatePostCommand.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(updatePostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        mockPostRepository.Setup(x => x.UpdateAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var updatePostCommandHandler = new UpdatePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePostCommandHandler.Handle(updatePostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<SaveError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new SaveError()]);
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Title, x => x.Random.String2(3, 25))
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var post = fakerPost.Generate();
        var postDto = mapper.Map<Post, PostDto>(post);

        var fakerUpdatePostCommand = new AutoFaker<UpdatePostCommand>()
            .RuleFor(x => x.Guid, _ => post.Guid)
            .RuleFor(x => x.Title, _ => post.Title)
            .RuleFor(x => x.Text, _ => post.Text)
            .RuleFor(x => x.Visible, _ => post.Visible);

        var updatePostCommand = fakerUpdatePostCommand.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(updatePostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        mockPostRepository.Setup(x => x.UpdateAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var updatePostCommandHandler = new UpdatePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePostCommandHandler.Handle(updatePostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
    }
}
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

public class DeletePostCommandTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IMediator mediator;
    private IValidator<DeletePostCommand> validator;
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
        validator = serviceProvider.GetRequiredService<IValidator<DeletePostCommand>>();
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
        var deletePostCommand = new AutoFaker<DeletePostCommand>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .Generate();

        var validationResult = validator.Validate(deletePostCommand);
        var validationResultErrors = validationResult.GetValidationErrors();

        var deletePostCommandHandler = new DeletePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await deletePostCommandHandler.Handle(deletePostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_NotFoundError()
    {
        // Arrange
        var deletePostCommand = new AutoFaker<DeletePostCommand>()
            .Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(deletePostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Post>());

        var deletePostCommandHandler = new DeletePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await deletePostCommandHandler.Handle(deletePostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<NotFoundError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new NotFoundError()]);
    }

    [Test]
    public async Task Should_Be_Invalid_When_SaveError()
    {
        // Arrange
        var deletePostCommand = new AutoFaker<DeletePostCommand>()
            .Generate();

        var post = new AutoFaker<Post>()
            .RuleFor(x => x.Guid, _ => deletePostCommand.Guid)
            .Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(deletePostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        mockPostRepository.Setup(x => x.DeleteAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var deletePostCommandHandler = new DeletePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await deletePostCommandHandler.Handle(deletePostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<SaveError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new SaveError()]);
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var deletePostCommand = new AutoFaker<DeletePostCommand>()
            .Generate();

        var post = new AutoFaker<Post>()
            .RuleFor(x => x.Guid, _ => deletePostCommand.Guid)
            .Generate();

        var postDto = mapper.Map<Post, PostDto>(post);

        mockPostRepository.Setup(x => x.GetByGuidAsync(deletePostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        mockPostRepository.Setup(x => x.DeleteAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var deletePostCommandHandler = new DeletePostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await deletePostCommandHandler.Handle(deletePostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
    }
}
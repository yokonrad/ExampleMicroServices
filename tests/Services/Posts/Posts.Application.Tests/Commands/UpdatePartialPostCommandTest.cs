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

public class UpdatePartialPostCommandTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IMediator mediator;
    private IValidator<UpdatePartialPostCommand> validator;
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
        validator = serviceProvider.GetRequiredService<IValidator<UpdatePartialPostCommand>>();
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
        var fakerUpdatePartialPostCommand = new AutoFaker<UpdatePartialPostCommand>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>());

        var updatePartialPostCommand = fakerUpdatePartialPostCommand.Generate();

        var validationResult = validator.Validate(updatePartialPostCommand);
        var validationResultErrors = validationResult.GetValidationErrors();

        var updatePartialPostCommandHandler = new UpdatePartialPostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePartialPostCommandHandler.Handle(updatePartialPostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_NotFoundError()
    {
        // Arrange
        var fakerUpdatePartialPostCommand = new AutoFaker<UpdatePartialPostCommand>()
            .RuleFor(x => x.Title, x => x.Random.String2(3, 25))
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var updatePartialPostCommand = fakerUpdatePartialPostCommand.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(updatePartialPostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Post>());

        var updatePartialPostCommandHandler = new UpdatePartialPostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePartialPostCommandHandler.Handle(updatePartialPostCommand, It.IsAny<CancellationToken>());

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

        var fakerUpdatePartialPostCommand = new AutoFaker<UpdatePartialPostCommand>()
            .RuleFor(x => x.Guid, _ => post.Guid)
            .RuleFor(x => x.Title, _ => post.Title)
            .RuleFor(x => x.Text, _ => post.Text)
            .RuleFor(x => x.Visible, _ => post.Visible);

        var updatePartialPostCommand = fakerUpdatePartialPostCommand.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(updatePartialPostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        mockPostRepository.Setup(x => x.UpdateAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var updatePartialPostCommandHandler = new UpdatePartialPostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePartialPostCommandHandler.Handle(updatePartialPostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<SaveError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new SaveError()]);
    }

    [Test]
    public async Task Should_Be_Valid_When_Title_Null()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Title, x => x.Random.String2(3, 25))
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var post = fakerPost.Generate();
        var postDto = mapper.Map<Post, PostDto>(post);

        var fakerUpdatePartialPostCommand = new AutoFaker<UpdatePartialPostCommand>()
            .RuleFor(x => x.Guid, _ => post.Guid)
            .RuleFor(x => x.Title, _ => null)
            .RuleFor(x => x.Text, _ => post.Text)
            .RuleFor(x => x.Visible, _ => post.Visible);

        var updatePartialPostCommand = fakerUpdatePartialPostCommand.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(updatePartialPostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        mockPostRepository.Setup(x => x.UpdateAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var updatePartialPostCommandHandler = new UpdatePartialPostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePartialPostCommandHandler.Handle(updatePartialPostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
    }

    [Test]
    public async Task Should_Be_Valid_When_Text_Null()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Title, x => x.Random.String2(3, 25))
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var post = fakerPost.Generate();
        var postDto = mapper.Map<Post, PostDto>(post);

        var fakerUpdatePartialPostCommand = new AutoFaker<UpdatePartialPostCommand>()
            .RuleFor(x => x.Guid, _ => post.Guid)
            .RuleFor(x => x.Title, _ => post.Title)
            .RuleFor(x => x.Text, _ => null)
            .RuleFor(x => x.Visible, _ => post.Visible);

        var updatePartialPostCommand = fakerUpdatePartialPostCommand.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(updatePartialPostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        mockPostRepository.Setup(x => x.UpdateAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var updatePartialPostCommandHandler = new UpdatePartialPostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePartialPostCommandHandler.Handle(updatePartialPostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
    }

    [Test]
    public async Task Should_Be_Valid_When_Visible_Null()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Title, x => x.Random.String2(3, 25))
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var post = fakerPost.Generate();
        var postDto = mapper.Map<Post, PostDto>(post);

        var fakerUpdatePartialPostCommand = new AutoFaker<UpdatePartialPostCommand>()
            .RuleFor(x => x.Guid, _ => post.Guid)
            .RuleFor(x => x.Title, _ => post.Title)
            .RuleFor(x => x.Text, _ => post.Text)
            .RuleFor(x => x.Visible, _ => null);

        var updatePartialPostCommand = fakerUpdatePartialPostCommand.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(updatePartialPostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        mockPostRepository.Setup(x => x.UpdateAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var updatePartialPostCommandHandler = new UpdatePartialPostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePartialPostCommandHandler.Handle(updatePartialPostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
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

        var fakerUpdatePartialPostCommand = new AutoFaker<UpdatePartialPostCommand>()
            .RuleFor(x => x.Guid, _ => post.Guid)
            .RuleFor(x => x.Title, _ => post.Title)
            .RuleFor(x => x.Text, _ => post.Text)
            .RuleFor(x => x.Visible, _ => post.Visible);

        var updatePartialPostCommand = fakerUpdatePartialPostCommand.Generate();

        mockPostRepository.Setup(x => x.GetByGuidAsync(updatePartialPostCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        mockPostRepository.Setup(x => x.UpdateAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var updatePartialPostCommandHandler = new UpdatePartialPostCommandHandler(mapper, mediator, validator, mockPostRepository.Object);

        // Act
        var act = await updatePartialPostCommandHandler.Handle(updatePartialPostCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<PostDto>().And.NotBeNull().And.BeEquivalentTo(postDto);
    }
}
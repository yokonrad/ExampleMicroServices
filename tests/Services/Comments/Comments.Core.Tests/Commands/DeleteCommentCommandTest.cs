using AutoBogus;
using AutoMapper;
using Comments.Core.Commands;
using Comments.Core.Dtos;
using Comments.Core.Entities;
using Comments.Core.Extensions;
using Comments.Core.Interfaces;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Comments.Core.Tests.Commands;

public class DeleteCommentCommandTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IMediator mediator;
    private IValidator<DeleteCommentCommand> validator;
    private Mock<ICommentRepository> mockCommentRepository;
    private Mock<IPostService> mockPostService;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCommentsCoreSupport(x => x.UseInMemoryDatabase("DbConnection"));
        serviceCollection.AddLogging(x => x.AddProvider(NullLoggerProvider.Instance));

        serviceProvider = serviceCollection.BuildServiceProvider();
        mapper = serviceProvider.GetRequiredService<IMapper>();
        mediator = serviceProvider.GetRequiredService<IMediator>();
        validator = serviceProvider.GetRequiredService<IValidator<DeleteCommentCommand>>();
        mockCommentRepository = new Mock<ICommentRepository>();
        mockPostService = new Mock<IPostService>();
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
        var fakerDeleteCommentCommand = new AutoFaker<DeleteCommentCommand>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>());

        var deleteCommentCommand = fakerDeleteCommentCommand.Generate();

        var validationResult = validator.Validate(deleteCommentCommand);
        var validationResultErrors = validationResult.GetValidationErrors();

        var deleteCommentCommandHandler = new DeleteCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await deleteCommentCommandHandler.Handle(deleteCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_NotFoundError()
    {
        // Arrange
        var fakerDeleteCommentCommand = new AutoFaker<DeleteCommentCommand>();
        var deleteCommentCommand = fakerDeleteCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(deleteCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Comment>());

        var deleteCommentCommandHandler = new DeleteCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await deleteCommentCommandHandler.Handle(deleteCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<NotFoundError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new NotFoundError()]);
    }

    [Test]
    public async Task Should_Be_Invalid_When_ServiceError()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>();
        var comment = fakerComment.Generate();

        var fakerDeleteCommentCommand = new AutoFaker<DeleteCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid);

        var deleteCommentCommand = fakerDeleteCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(deleteCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(comment.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<PostDto>());

        var deleteCommentCommandHandler = new DeleteCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await deleteCommentCommandHandler.Handle(deleteCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ServiceError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new ServiceError()]);
    }

    [Test]
    public async Task Should_Be_Invalid_When_SaveError()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid);

        var comment = fakerComment.Generate();

        var fakerDeleteCommentCommand = new AutoFaker<DeleteCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid);

        var deleteCommentCommand = fakerDeleteCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(deleteCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(comment.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.DeleteAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var deleteCommentCommandHandler = new DeleteCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await deleteCommentCommandHandler.Handle(deleteCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<SaveError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new SaveError()]);
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid);

        var comment = fakerComment.Generate();
        var commentDto = mapper.Map<CommentDto>(comment);

        var fakerDeleteCommentCommand = new AutoFaker<DeleteCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid);

        var deleteCommentCommand = fakerDeleteCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(deleteCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(comment.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.DeleteAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var deleteCommentCommandHandler = new DeleteCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await deleteCommentCommandHandler.Handle(deleteCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<CommentDto>().And.NotBeNull().And.BeEquivalentTo(commentDto);
    }
}
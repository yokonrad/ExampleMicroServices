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

public class UpdateCommentCommandTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IMediator mediator;
    private IValidator<UpdateCommentCommand> validator;
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
        validator = serviceProvider.GetRequiredService<IValidator<UpdateCommentCommand>>();
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
        var fakerUpdateCommentCommand = new AutoFaker<UpdateCommentCommand>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.PostGuid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>());

        var updateCommentCommand = fakerUpdateCommentCommand.Generate();

        var validationResult = validator.Validate(updateCommentCommand);
        var validationResultErrors = validationResult.GetValidationErrors();

        var updateCommentCommandHandler = new UpdateCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updateCommentCommandHandler.Handle(updateCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_NotFoundError()
    {
        // Arrange
        var fakerUpdateCommentCommand = new AutoFaker<UpdateCommentCommand>()
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var updateCommentCommand = fakerUpdateCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updateCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Comment>());

        var updateCommentCommandHandler = new UpdateCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updateCommentCommandHandler.Handle(updateCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<NotFoundError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new NotFoundError()]);
    }

    [Test]
    public async Task Should_Be_Invalid_When_ServiceError()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid)
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var comment = fakerComment.Generate();

        var fakerUpdateCommentCommand = new AutoFaker<UpdateCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid)
            .RuleFor(x => x.PostGuid, _ => comment.PostGuid)
            .RuleFor(x => x.Text, _ => comment.Text)
            .RuleFor(x => x.Visible, _ => comment.Visible);

        var updateCommentCommand = fakerUpdateCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updateCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(updateCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<PostDto>());

        var updateCommentCommandHandler = new UpdateCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updateCommentCommandHandler.Handle(updateCommentCommand, It.IsAny<CancellationToken>());

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
            .RuleFor(x => x.PostGuid, _ => postDto.Guid)
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var comment = fakerComment.Generate();

        var fakerUpdateCommentCommand = new AutoFaker<UpdateCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid)
            .RuleFor(x => x.PostGuid, _ => comment.PostGuid)
            .RuleFor(x => x.Text, _ => comment.Text)
            .RuleFor(x => x.Visible, _ => comment.Visible);

        var updateCommentCommand = fakerUpdateCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updateCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(updateCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.UpdateAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var updateCommentCommandHandler = new UpdateCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updateCommentCommandHandler.Handle(updateCommentCommand, It.IsAny<CancellationToken>());

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
            .RuleFor(x => x.PostGuid, _ => postDto.Guid)
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var comment = fakerComment.Generate();
        var commentDto = mapper.Map<CommentDto>(comment);

        var fakerUpdateCommentCommand = new AutoFaker<UpdateCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid)
            .RuleFor(x => x.PostGuid, _ => comment.PostGuid)
            .RuleFor(x => x.Text, _ => comment.Text)
            .RuleFor(x => x.Visible, _ => comment.Visible);

        var updateCommentCommand = fakerUpdateCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updateCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(updateCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.UpdateAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var updateCommentCommandHandler = new UpdateCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updateCommentCommandHandler.Handle(updateCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<CommentDto>().And.NotBeNull().And.BeEquivalentTo(commentDto);
    }
}
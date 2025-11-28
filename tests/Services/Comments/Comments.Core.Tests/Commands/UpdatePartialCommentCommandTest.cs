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

public class UpdatePartialCommentCommandTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IMediator mediator;
    private IValidator<UpdatePartialCommentCommand> validator;
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
        validator = serviceProvider.GetRequiredService<IValidator<UpdatePartialCommentCommand>>();
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
        var fakerUpdatePartialCommentCommand = new AutoFaker<UpdatePartialCommentCommand>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>());

        var updatePartialCommentCommand = fakerUpdatePartialCommentCommand.Generate();

        var validationResult = validator.Validate(updatePartialCommentCommand);
        var validationResultErrors = validationResult.GetValidationErrors();

        var updatePartialCommentCommandHandler = new UpdatePartialCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updatePartialCommentCommandHandler.Handle(updatePartialCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_NotFoundError()
    {
        // Arrange
        var fakerUpdatePartialCommentCommand = new AutoFaker<UpdatePartialCommentCommand>()
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var updatePartialCommentCommand = fakerUpdatePartialCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Comment>());

        var updatePartialCommentCommandHandler = new UpdatePartialCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updatePartialCommentCommandHandler.Handle(updatePartialCommentCommand, It.IsAny<CancellationToken>());

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

        var fakerUpdatePartialCommentCommand = new AutoFaker<UpdatePartialCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid)
            .RuleFor(x => x.PostGuid, _ => comment.PostGuid)
            .RuleFor(x => x.Text, _ => comment.Text)
            .RuleFor(x => x.Visible, _ => comment.Visible);

        var updatePartialCommentCommand = fakerUpdatePartialCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<PostDto>());

        var updatePartialCommentCommandHandler = new UpdatePartialCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updatePartialCommentCommandHandler.Handle(updatePartialCommentCommand, It.IsAny<CancellationToken>());

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

        var fakerUpdatePartialCommentCommand = new AutoFaker<UpdatePartialCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid)
            .RuleFor(x => x.PostGuid, _ => comment.PostGuid)
            .RuleFor(x => x.Text, _ => comment.Text)
            .RuleFor(x => x.Visible, _ => comment.Visible);

        var updatePartialCommentCommand = fakerUpdatePartialCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.UpdateAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var updatePartialCommentCommandHandler = new UpdatePartialCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updatePartialCommentCommandHandler.Handle(updatePartialCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<SaveError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new SaveError()]);
    }

    [Test]
    public async Task Should_Be_Valid_When_Text_Null()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid)
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var comment = fakerComment.Generate();
        var commentDto = mapper.Map<CommentDto>(comment);

        var fakerUpdatePartialCommentCommand = new AutoFaker<UpdatePartialCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid)
            .RuleFor(x => x.PostGuid, _ => comment.PostGuid)
            .RuleFor(x => x.Text, _ => null)
            .RuleFor(x => x.Visible, _ => comment.Visible);

        var updatePartialCommentCommand = fakerUpdatePartialCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.UpdateAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var updatePartialCommentCommandHandler = new UpdatePartialCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updatePartialCommentCommandHandler.Handle(updatePartialCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<CommentDto>().And.NotBeNull().And.BeEquivalentTo(commentDto);
    }

    [Test]
    public async Task Should_Be_Valid_When_Visible_Null()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid)
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var comment = fakerComment.Generate();
        var commentDto = mapper.Map<Comment, CommentDto>(comment);

        var fakerUpdatePartialCommentCommand = new AutoFaker<UpdatePartialCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid)
            .RuleFor(x => x.PostGuid, _ => comment.PostGuid)
            .RuleFor(x => x.Text, _ => comment.Text)
            .RuleFor(x => x.Visible, _ => null);

        var updatePartialCommentCommand = fakerUpdatePartialCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.UpdateAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var updatePartialCommentCommandHandler = new UpdatePartialCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updatePartialCommentCommandHandler.Handle(updatePartialCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<CommentDto>().And.NotBeNull().And.BeEquivalentTo(commentDto);
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

        var fakerUpdatePartialCommentCommand = new AutoFaker<UpdatePartialCommentCommand>()
            .RuleFor(x => x.Guid, _ => comment.Guid)
            .RuleFor(x => x.PostGuid, _ => comment.PostGuid)
            .RuleFor(x => x.Text, _ => comment.Text)
            .RuleFor(x => x.Visible, _ => comment.Visible);

        var updatePartialCommentCommand = fakerUpdatePartialCommentCommand.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(updatePartialCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.UpdateAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var updatePartialCommentCommandHandler = new UpdatePartialCommentCommandHandler(mapper, mediator, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await updatePartialCommentCommandHandler.Handle(updatePartialCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<CommentDto>().And.NotBeNull().And.BeEquivalentTo(commentDto);
    }
}
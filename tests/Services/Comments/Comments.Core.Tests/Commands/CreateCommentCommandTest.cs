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

public class CreateCommentCommandTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IMediator mediator;
    private IValidator<CreateCommentCommand> validator;
    private Mock<IPostService> mockPostService;
    private Mock<ICommentRepository> mockCommentRepository;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCommentsCoreSupport(x => x.UseInMemoryDatabase("DbConnection"));
        serviceCollection.AddLogging(x => x.AddProvider(NullLoggerProvider.Instance));

        serviceProvider = serviceCollection.BuildServiceProvider();
        mapper = serviceProvider.GetRequiredService<IMapper>();
        mediator = serviceProvider.GetRequiredService<IMediator>();
        validator = serviceProvider.GetRequiredService<IValidator<CreateCommentCommand>>();
        mockPostService = new Mock<IPostService>();
        mockCommentRepository = new Mock<ICommentRepository>();
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
        var fakerCreateCommentCommand = new AutoFaker<CreateCommentCommand>()
            .RuleFor(x => x.PostGuid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>());

        var createCommentCommand = fakerCreateCommentCommand.Generate();

        var validationResult = validator.Validate(createCommentCommand);
        var validationResultErrors = validationResult.GetValidationErrors();

        var createCommentCommandHandler = new CreateCommentCommandHandler(mapper, mediator, validator, mockPostService.Object, mockCommentRepository.Object);

        // Act
        var act = await createCommentCommandHandler.Handle(createCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_ServiceError()
    {
        // Arrange
        var fakerCreateCommentCommand = new AutoFaker<CreateCommentCommand>()
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var createCommentCommand = fakerCreateCommentCommand.Generate();

        mockPostService.Setup(x => x.GetByGuidAsync(createCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<PostDto>());

        var createCommentCommandHandler = new CreateCommentCommandHandler(mapper, mediator, validator, mockPostService.Object, mockCommentRepository.Object);

        // Act
        var act = await createCommentCommandHandler.Handle(createCommentCommand, It.IsAny<CancellationToken>());

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

        var fakerCreateCommentCommand = new AutoFaker<CreateCommentCommand>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid)
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var createCommentCommand = fakerCreateCommentCommand.Generate();

        var comment = mapper.Map<CreateCommentCommand, Comment>(createCommentCommand);

        mockPostService.Setup(x => x.GetByGuidAsync(createCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.CreateAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var createCommentCommandHandler = new CreateCommentCommandHandler(mapper, mediator, validator, mockPostService.Object, mockCommentRepository.Object);

        // Act
        var act = await createCommentCommandHandler.Handle(createCommentCommand, It.IsAny<CancellationToken>());

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

        var fakerCreateCommentCommand = new AutoFaker<CreateCommentCommand>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid)
            .RuleFor(x => x.Text, x => x.Random.String2(3, 25));

        var createCommentCommand = fakerCreateCommentCommand.Generate();

        var comment = mapper.Map<CreateCommentCommand, Comment>(createCommentCommand);
        var commentDto = mapper.Map<Comment, CommentDto>(comment);

        mockPostService.Setup(x => x.GetByGuidAsync(createCommentCommand.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.CreateAsync(comment, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var createCommentCommandHandler = new CreateCommentCommandHandler(mapper, mediator, validator, mockPostService.Object, mockCommentRepository.Object);

        // Act
        var act = await createCommentCommandHandler.Handle(createCommentCommand, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<CommentDto>().And.NotBeNull().And.BeEquivalentTo(commentDto);
    }
}
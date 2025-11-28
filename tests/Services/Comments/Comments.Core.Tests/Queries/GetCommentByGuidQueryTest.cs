using AutoBogus;
using AutoMapper;
using Comments.Core.Dtos;
using Comments.Core.Entities;
using Comments.Core.Extensions;
using Comments.Core.Interfaces;
using Comments.Core.Queries;
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Comments.Core.Tests.Queries;

public class GetCommentByGuidQueryTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IValidator<GetCommentByGuidQuery> validator;
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
        validator = serviceProvider.GetRequiredService<IValidator<GetCommentByGuidQuery>>();
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
        var fakerGetCommentByGuidQuery = new AutoFaker<GetCommentByGuidQuery>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>());

        var getCommentByGuidQuery = fakerGetCommentByGuidQuery.Generate();

        var validationResult = validator.Validate(getCommentByGuidQuery);
        var validationResultErrors = validationResult.GetValidationErrors();

        var getCommentByGuidQueryHandler = new GetCommentByGuidQueryHandler(mapper, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await getCommentByGuidQueryHandler.Handle(getCommentByGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_NotFoundError()
    {
        // Arrange
        var fakerGetCommentByGuidQuery = new AutoFaker<GetCommentByGuidQuery>();
        var getCommentByGuidQuery = fakerGetCommentByGuidQuery.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(getCommentByGuidQuery.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<Comment>());

        var getCommentByGuidQueryHandler = new GetCommentByGuidQueryHandler(mapper, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await getCommentByGuidQueryHandler.Handle(getCommentByGuidQuery, It.IsAny<CancellationToken>());

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

        var fakerGetCommentByGuidQuery = new AutoFaker<GetCommentByGuidQuery>()
            .RuleFor(x => x.Guid, _ => comment.Guid);

        var getCommentByGuidQuery = fakerGetCommentByGuidQuery.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(getCommentByGuidQuery.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(comment.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<PostDto>());

        var getCommentByGuidQueryHandler = new GetCommentByGuidQueryHandler(mapper, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await getCommentByGuidQueryHandler.Handle(getCommentByGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ServiceError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new ServiceError()]);
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakeComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid);

        var comment = fakeComment.Generate();
        var commentDto = mapper.Map<Comment, CommentDto>(comment);

        var fakerGetCommentByGuidQuery = new AutoFaker<GetCommentByGuidQuery>()
            .RuleFor(x => x.Guid, _ => comment.Guid);

        var getCommentByGuidQuery = fakerGetCommentByGuidQuery.Generate();

        mockCommentRepository.Setup(x => x.GetByGuidAsync(getCommentByGuidQuery.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comment);
        mockPostService.Setup(x => x.GetByGuidAsync(comment.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);

        var getCommentByGuidQueryHandler = new GetCommentByGuidQueryHandler(mapper, validator, mockCommentRepository.Object, mockPostService.Object);

        // Act
        var act = await getCommentByGuidQueryHandler.Handle(getCommentByGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeOfType<CommentDto>().And.NotBeNull().And.BeEquivalentTo(commentDto);
    }
}
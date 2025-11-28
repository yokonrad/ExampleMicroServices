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

public class GetCommentsByPostGuidQueryTest
{
    private ServiceProvider serviceProvider;
    private IMapper mapper;
    private IValidator<GetCommentsByPostGuidQuery> validator;
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
        validator = serviceProvider.GetRequiredService<IValidator<GetCommentsByPostGuidQuery>>();
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
        var fakerGetCommentsByPostGuidQuery = new AutoFaker<GetCommentsByPostGuidQuery>()
            .RuleFor(x => x.PostGuid, _ => It.IsAny<Guid>());

        var getCommentsByPostGuidQuery = fakerGetCommentsByPostGuidQuery.Generate();

        var validationResult = validator.Validate(getCommentsByPostGuidQuery);
        var validationResultErrors = validationResult.GetValidationErrors();

        var getCommentsByPostGuidQueryHandler = new GetCommentsByPostGuidQueryHandler(mapper, validator, mockPostService.Object, mockCommentRepository.Object);

        // Act
        var act = await getCommentsByPostGuidQueryHandler.Handle(fakerGetCommentsByPostGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ValidationError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo(validationResultErrors);
    }

    [Test]
    public async Task Should_Be_Invalid_When_ServiceError()
    {
        // Arrange
        var fakerGetCommentsByPostGuidQuery = new AutoFaker<GetCommentsByPostGuidQuery>();
        var getCommentsByPostGuidQuery = fakerGetCommentsByPostGuidQuery.Generate();

        mockPostService.Setup(x => x.GetByGuidAsync(getCommentsByPostGuidQuery.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<PostDto>());

        var getCommentsByPostGuidQueryHandler = new GetCommentsByPostGuidQueryHandler(mapper, validator, mockPostService.Object, mockCommentRepository.Object);

        // Act
        var act = await getCommentsByPostGuidQueryHandler.Handle(getCommentsByPostGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsFailed.Should().BeTrue();
        act.HasError<ServiceError>(out var actErrors).Should().BeTrue();
        actErrors.Should().NotBeNull().And.NotBeEmpty().And.BeEquivalentTo([new ServiceError()]);
    }

    [Test]
    public async Task Should_Be_Valid_When_Empty()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var comments = Array.Empty<Comment>();
        var commentDtos = mapper.Map<IEnumerable<CommentDto>>(comments);

        var fakerGetCommentsByPostGuidQuery = new AutoFaker<GetCommentsByPostGuidQuery>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid);

        var getCommentsByPostGuidQuery = fakerGetCommentsByPostGuidQuery.Generate();

        mockPostService.Setup(x => x.GetByGuidAsync(getCommentsByPostGuidQuery.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.GetByPostGuidAsync(postDto.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comments);

        var getCommentsByPostGuidQueryHandler = new GetCommentsByPostGuidQueryHandler(mapper, validator, mockPostService.Object, mockCommentRepository.Object);

        // Act
        var act = await getCommentsByPostGuidQueryHandler.Handle(getCommentsByPostGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeAssignableTo<IEnumerable<CommentDto>>().And.BeEquivalentTo(commentDtos).And.BeEmpty();
    }

    [Test]
    public async Task Should_Be_Valid_When_Not_Empty()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakeComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid);

        var comments = fakeComment.Generate(100).ToArray();
        var commentDtos = mapper.Map<IEnumerable<CommentDto>>(comments);

        var fakerGetCommentsByPostGuidQuery = new AutoFaker<GetCommentsByPostGuidQuery>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid);

        var getCommentsByPostGuidQuery = fakerGetCommentsByPostGuidQuery.Generate();

        mockPostService.Setup(x => x.GetByGuidAsync(getCommentsByPostGuidQuery.PostGuid, It.IsAny<CancellationToken>())).ReturnsAsync(postDto);
        mockCommentRepository.Setup(x => x.GetByPostGuidAsync(postDto.Guid, It.IsAny<CancellationToken>())).ReturnsAsync(comments);

        var getCommentsByPostGuidQueryHandler = new GetCommentsByPostGuidQueryHandler(mapper, validator, mockPostService.Object, mockCommentRepository.Object);

        // Act
        var act = await getCommentsByPostGuidQueryHandler.Handle(getCommentsByPostGuidQuery, It.IsAny<CancellationToken>());

        // Assert
        act.IsSuccess.Should().BeTrue();
        act.Value.Should().BeAssignableTo<IEnumerable<CommentDto>>().And.BeEquivalentTo(commentDtos).And.NotBeEmpty();
    }
}
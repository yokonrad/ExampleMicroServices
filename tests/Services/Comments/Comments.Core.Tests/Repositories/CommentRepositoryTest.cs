using AutoBogus;
using Comments.Core.Data;
using Comments.Core.Dtos;
using Comments.Core.Entities;
using Comments.Core.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Testcontainers.MsSql;

namespace Comments.Core.Tests.Repositories;

public class CommentRepositoryTest
{
    private MsSqlContainer msSqlContainer;
    private CommentDbContext commentDbContext;
    private CommentRepository commentRepository;

    [SetUp]
    public async Task SetUp()
    {
        msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

        await msSqlContainer.StartAsync();

        commentDbContext = new CommentDbContext(new DbContextOptionsBuilder<CommentDbContext>().UseSqlServer(msSqlContainer.GetConnectionString()).Options);

        await commentDbContext.Database.EnsureCreatedAsync();

        commentRepository = new CommentRepository(commentDbContext);
    }

    [TearDown]
    public async Task TearDown()
    {
        await commentDbContext.DisposeAsync();
        await msSqlContainer.DisposeAsync();
    }

    [Test]
    public async Task GetByGuidAsync_Should_Throw_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act
        var act = () => commentRepository.GetByGuidAsync(It.IsAny<Guid>(), cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task GetByGuidAsync_Should_Be_Invalid()
    {
        // Act
        var act = await commentRepository.GetByGuidAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeNull();
    }

    [Test]
    public async Task GetByGuidAsync_Should_Be_Valid()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>();
        var comment = fakerComment.Generate();

        commentDbContext.Comments.Add(comment);
        await commentDbContext.SaveChangesAsync(It.IsAny<CancellationToken>());

        // Act
        var act = await commentRepository.GetByGuidAsync(comment.Guid, It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeOfType<Comment>().And.NotBeNull().And.BeEquivalentTo(comment);
    }

    [Test]
    public async Task GetByPostGuidAsync_Should_Be_Valid_When_Empty()
    {
        // Arrange
        var comments = Array.Empty<Comment>();

        // Act
        var act = await commentRepository.GetByPostGuidAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeOfType<Comment[]>().And.BeEquivalentTo(comments).And.BeEmpty();
    }

    [Test]
    public async Task GetByPostGuidAsync_Should_Be_Valid_When_Not_Empty()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid);

        var comments = fakerComment.Generate(100);

        commentDbContext.Comments.AddRange(comments);
        await commentDbContext.SaveChangesAsync(It.IsAny<CancellationToken>());

        // Act
        var act = await commentRepository.GetByPostGuidAsync(postDto.Guid, It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeOfType<Comment[]>().And.BeEquivalentTo(comments).And.NotBeEmpty();
    }

    [Test]
    public async Task CreateAsync_Should_Throw_NullReferenceException_When_Any_Comment()
    {
        // Act
        var act = () => commentRepository.CreateAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public async Task CreateAsync_Should_Throw_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>();
        var comment = fakerComment.Generate();

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act
        var act = () => commentRepository.CreateAsync(comment, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task CreateAsync_Should_Be_Valid()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>();
        var comment = fakerComment.Generate();

        // Act
        var act = await commentRepository.CreateAsync(comment, It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeTrue();
    }

    [Test]
    public async Task UpdateAsync_Should_Throw_NullReferenceException_When_Any_Comment()
    {
        // Act
        var act = () => commentRepository.UpdateAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public async Task UpdateAsync_Should_Throw_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>();
        var comment = fakerComment.Generate();

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act
        var act = () => commentRepository.UpdateAsync(comment, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task UpdateAsync_Should_Be_Valid()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>();
        var comment = fakerComment.Generate();

        commentDbContext.Comments.Add(comment);
        await commentDbContext.SaveChangesAsync(It.IsAny<CancellationToken>());

        // Act
        var act = await commentRepository.UpdateAsync(comment, It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeTrue();
    }

    [Test]
    public async Task DeleteAsync_Should_Throw_ArgumentNullException_When_Any_Comment()
    {
        // Act
        var act = () => commentRepository.DeleteAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task DeleteAsync_Should_Throw_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>();
        var comment = fakerComment.Generate();

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act
        var act = () => commentRepository.DeleteAsync(comment, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task DeleteAsync_Should_Be_Valid()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>();
        var comment = fakerComment.Generate();

        commentDbContext.Comments.Add(comment);
        await commentDbContext.SaveChangesAsync(It.IsAny<CancellationToken>());

        // Act
        var act = await commentRepository.DeleteAsync(comment, It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeTrue();
    }
}
using AutoBogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Posts.Domain.Entities;
using Posts.Infrastructure.Data;
using Posts.Infrastructure.Repositories;
using Testcontainers.MsSql;

namespace Posts.Infrastructure.Tests.Repositories;

public class PostRepositoryTest
{
    private MsSqlContainer msSqlContainer;
    private PostDbContext postDbContext;
    private PostRepository postRepository;

    [SetUp]
    public async Task SetUp()
    {
        msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

        await msSqlContainer.StartAsync();

        postDbContext = new PostDbContext(new DbContextOptionsBuilder<PostDbContext>().UseSqlServer(msSqlContainer.GetConnectionString()).Options);

        await postDbContext.Database.EnsureCreatedAsync();

        postRepository = new PostRepository(postDbContext);
    }

    [TearDown]
    public async Task TearDown()
    {
        await postDbContext.DisposeAsync();
        await msSqlContainer.DisposeAsync();
    }

    [Test]
    public async Task GetAsync_Should_Throw_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act
        var act = () => postRepository.GetAsync(cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task GetAsync_Should_Be_Valid()
    {
        // Arrange & Act
        var act = await postRepository.GetAsync(It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeOfType<Post[]>();
    }

    [Test]
    public async Task GetByGuidAsync_Should_Throw_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act
        var act = () => postRepository.GetByGuidAsync(It.IsAny<Guid>(), cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task GetByGuidAsync_Should_Be_Invalid()
    {
        // Act
        var act = await postRepository.GetByGuidAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeNull();
    }

    [Test]
    public async Task GetByGuidAsync_Should_Be_Valid()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();
        var post = fakerPost.Generate();

        postDbContext.Posts.Add(post);
        await postDbContext.SaveChangesAsync(It.IsAny<CancellationToken>());

        // Act
        var act = await postRepository.GetByGuidAsync(post.Guid, It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeOfType<Post>().And.NotBeNull().And.BeEquivalentTo(post);
    }

    [Test]
    public async Task CreateAsync_Should_Throw_NullReferenceException_When_Any_Post()
    {
        // Act
        var act = () => postRepository.CreateAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public async Task CreateAsync_Should_Throw_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();
        var post = fakerPost.Generate();

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act
        var act = () => postRepository.CreateAsync(post, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task CreateAsync_Should_Be_Valid()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();
        var post = fakerPost.Generate();

        // Act
        var act = await postRepository.CreateAsync(post, It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeTrue();
    }

    [Test]
    public async Task UpdateAsync_Should_Throw_NullReferenceException_When_Any_Post()
    {
        // Act
        var act = () => postRepository.UpdateAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Test]
    public async Task UpdateAsync_Should_Throw_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();
        var post = fakerPost.Generate();

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act
        var act = () => postRepository.UpdateAsync(post, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task UpdateAsync_Should_Be_Valid()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();
        var post = fakerPost.Generate();

        postDbContext.Posts.Add(post);
        await postDbContext.SaveChangesAsync(It.IsAny<CancellationToken>());

        // Act
        var act = await postRepository.UpdateAsync(post, It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeTrue();
    }

    [Test]
    public async Task DeleteAsync_Should_Throw_ArgumentNullException_When_Any_Post()
    {
        // Act
        var act = () => postRepository.DeleteAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>());

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task DeleteAsync_Should_Throw_OperationCanceledException_When_Cancelled()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();
        var post = fakerPost.Generate();

        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        // Act
        var act = () => postRepository.DeleteAsync(post, cancellationTokenSource.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task DeleteAsync_Should_Be_Valid()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();
        var post = fakerPost.Generate();

        postDbContext.Posts.Add(post);
        await postDbContext.SaveChangesAsync(It.IsAny<CancellationToken>());

        // Act
        var act = await postRepository.DeleteAsync(post, It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeTrue();
    }
}
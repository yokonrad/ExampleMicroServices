using Comments.Core.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Comments.Core.Tests.Data;

public class CommentDbContextTest
{
    private MsSqlContainer msSqlContainer;
    private CommentDbContext commentDbContext;

    [SetUp]
    public async Task SetUp()
    {
        msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

        await msSqlContainer.StartAsync();

        commentDbContext = new CommentDbContext(new DbContextOptionsBuilder<CommentDbContext>().UseSqlServer(msSqlContainer.GetConnectionString()).Options);
    }

    [TearDown]
    public async Task TearDown()
    {
        await commentDbContext.DisposeAsync();
        await msSqlContainer.DisposeAsync();
    }

    [Test]
    public void EnsureCreated_Should_Be_Valid()
    {
        // Arrange & Act
        var act = commentDbContext.Database.EnsureCreated();

        // Assert
        act.Should().BeTrue();
    }

    [Test]
    public async Task EnsureCreatedAsync_Should_Be_Valid()
    {
        // Arrange & Act
        var act = await commentDbContext.Database.EnsureCreatedAsync();

        // Assert
        act.Should().BeTrue();
    }
}
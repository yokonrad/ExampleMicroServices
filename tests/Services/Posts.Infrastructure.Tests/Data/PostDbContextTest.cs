using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Posts.Infrastructure.Data;
using Testcontainers.MsSql;

namespace Posts.Infrastructure.Tests.Data;

public class PostDbContextTest
{
    private MsSqlContainer msSqlContainer;
    private PostDbContext postDbContext;

    [SetUp]
    public async Task SetUp()
    {
        msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

        await msSqlContainer.StartAsync();

        postDbContext = new PostDbContext(new DbContextOptionsBuilder<PostDbContext>().UseSqlServer(msSqlContainer.GetConnectionString()).Options);
    }

    [TearDown]
    public async Task TearDown()
    {
        await postDbContext.DisposeAsync();
        await msSqlContainer.DisposeAsync();
    }

    [Test]
    public void EnsureCreated_Should_Be_Valid()
    {
        // Arrange & Act
        var act = postDbContext.Database.EnsureCreated();

        // Assert
        act.Should().BeTrue();
    }

    [Test]
    public async Task EnsureCreatedAsync_Should_Be_Valid()
    {
        // Arrange & Act
        var act = await postDbContext.Database.EnsureCreatedAsync();

        // Assert
        act.Should().BeTrue();
    }
}
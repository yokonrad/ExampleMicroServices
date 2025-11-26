using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Posts.Domain.Interfaces;
using Posts.Infrastructure.Data;
using Posts.Infrastructure.Extensions;
using Posts.Infrastructure.Repositories;
using Testcontainers.MsSql;

namespace Posts.Infrastructure.Tests.Extensions;

public class DependencyInjectionExtensionTest
{
    private MsSqlContainer msSqlContainer;

    [SetUp]
    public async Task SetUp()
    {
        msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

        await msSqlContainer.StartAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await msSqlContainer.DisposeAsync();
    }

    [Test]
    public void AddPostsInfrastructureSupport_IServiceCollection_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPostsInfrastructureSupport(x => x.UseSqlServer(msSqlContainer.GetConnectionString()));

        // Act
        var act = serviceCollection.BuildServiceProvider();
        var actPostDbContextServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(PostDbContext).Name && x.ImplementationType?.Name == typeof(PostDbContext).Name && x.Lifetime == ServiceLifetime.Scoped);
        var actPostRepositoryServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IPostRepository).Name && x.ImplementationType?.Name == typeof(PostRepository).Name && x.Lifetime == ServiceLifetime.Scoped);

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actPostDbContextServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actPostRepositoryServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
    }

    [Test]
    public void AddPostsInfrastructureSupport_IServiceProvider_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPostsInfrastructureSupport(x => x.UseSqlServer(msSqlContainer.GetConnectionString()));

        // Act
        var act = serviceCollection.BuildServiceProvider();
        act.AddPostsInfrastructureSupport();

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
    }
}
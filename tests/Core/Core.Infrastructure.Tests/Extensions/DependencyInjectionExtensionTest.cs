using Core.Infrastructure.Extensions;
using Core.Infrastructure.Tests.Examples;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Core.Infrastructure.Tests.Extensions;

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
    public void AddEntityFrameworkCoreSupport_IServiceCollection_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkCoreSupport<ExampleDbContext>(x => x.UseSqlServer(msSqlContainer.GetConnectionString()));

        // Act
        var act = serviceCollection.BuildServiceProvider();
        var actExampleDbContextServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == nameof(ExampleDbContext) && x.ImplementationType?.Name == nameof(ExampleDbContext));

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actExampleDbContextServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
    }

    [Test]
    public void AddEntityFrameworkCoreSupport_IServiceProvider_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkCoreSupport<ExampleDbContext>(x => x.UseSqlServer(msSqlContainer.GetConnectionString()));

        var serviceProvider = serviceCollection.BuildServiceProvider();
        serviceProvider.AddEntityFrameworkCoreSupport<ExampleDbContext>();

        using var scope = serviceProvider.CreateScope();
        using var exampleDbContext = scope.ServiceProvider.GetRequiredService<ExampleDbContext>();

        // Act
        var act = exampleDbContext.Database.EnsureCreated();

        // Assert
        act.Should().BeFalse();
    }
}
using AutoMapper;
using Comments.Core.Commands;
using Comments.Core.Data;
using Comments.Core.Extensions;
using Comments.Core.Interfaces;
using Comments.Core.Repositories;
using Comments.Core.Services;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Comments.Core.Tests.Extensions;

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
    public void AddCommentsCoreSupport_IServiceCollection_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCommentsCoreSupport(x => x.UseSqlServer(msSqlContainer.GetConnectionString()));

        // Act
        var act = serviceCollection.BuildServiceProvider();
        var actCommentDbContextServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(CommentDbContext).Name && x.ImplementationType?.Name == typeof(CommentDbContext).Name);

        var actHttpClientFactoryServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IHttpClientFactory).Name);
        var actMapperServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IMapper).Name);

        var actCreateCommentCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(CreateCommentCommandValidator).Name);
        var actDeleteCommentCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(DeleteCommentCommandValidator).Name);

        var actCreateCommentCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(CreateCommentCommandHandler).Name);
        var actDeleteCommentCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(DeleteCommentCommandHandler).Name);

        var actCommentRepositoryServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(ICommentRepository).Name && x.ImplementationType?.Name == typeof(CommentRepository).Name);
        var actPostServiceServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IPostService).Name && x.ImplementationType?.Name == typeof(PostService).Name);

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actCommentDbContextServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();

        actHttpClientFactoryServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actMapperServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();

        actCreateCommentCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actDeleteCommentCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();

        actCreateCommentCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actDeleteCommentCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();

        actCommentRepositoryServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actPostServiceServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
    }

    [Test]
    public void AddCommentsCoreSupport_IServiceProvider_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCommentsCoreSupport(x => x.UseSqlServer(msSqlContainer.GetConnectionString()));

        // Act
        var act = serviceCollection.BuildServiceProvider();
        act.AddCommentsCoreSupport();

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
    }
}
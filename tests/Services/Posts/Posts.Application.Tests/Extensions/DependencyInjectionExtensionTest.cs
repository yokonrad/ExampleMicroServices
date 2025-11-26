using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Posts.Application.Commands;
using Posts.Application.Extensions;
using Posts.Application.Notifications;
using Posts.Application.Queries;

namespace Posts.Application.Tests.Extensions;

public class DependencyInjectionExtensionTest
{
    [Test]
    public void AddPostsApplicationSupport_IServiceCollection_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPostsApplicationSupport();

        // Act
        var act = serviceCollection.BuildServiceProvider();
        var actMapperServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IMapper).Name && x.Lifetime == ServiceLifetime.Transient);

        var actCreatePostCommandValidatorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(CreatePostCommandValidator).Name && x.Lifetime == ServiceLifetime.Scoped);
        var actDeletePostCommandValidatorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(DeletePostCommandValidator).Name && x.Lifetime == ServiceLifetime.Scoped);
        var actUpdatePartialPostCommandValidatorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(UpdatePartialPostCommandValidator).Name && x.Lifetime == ServiceLifetime.Scoped);
        var actUpdatePostCommandValidatorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(UpdatePostCommandValidator).Name && x.Lifetime == ServiceLifetime.Scoped);
        var actGetPostByGuidQueryValidatorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(GetPostByGuidQueryValidator).Name && x.Lifetime == ServiceLifetime.Scoped);
        var actGetPostsQueryValidatorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(GetPostsQueryValidator).Name && x.Lifetime == ServiceLifetime.Scoped);

        var actCreatePostCommandHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(CreatePostCommandHandler).Name && x.Lifetime == ServiceLifetime.Transient);
        var actDeletePostCommandHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(DeletePostCommandHandler).Name && x.Lifetime == ServiceLifetime.Transient);
        var actUpdatePartialPostCommandHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(UpdatePartialPostCommandHandler).Name && x.Lifetime == ServiceLifetime.Transient);
        var actUpdatePostCommandHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(UpdatePostCommandHandler).Name && x.Lifetime == ServiceLifetime.Transient);
        var actGetPostByGuidQueryHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(GetPostByGuidQueryHandler).Name && x.Lifetime == ServiceLifetime.Transient);
        var actGetPostsQueryHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(GetPostsQueryHandler).Name && x.Lifetime == ServiceLifetime.Transient);

        var actCreatedPostNotificationHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(INotificationHandler<>).Name && x.ImplementationType?.Name == typeof(CreatedPostNotificationHandler).Name && x.Lifetime == ServiceLifetime.Transient);
        var actDeletedPostNotificationHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(INotificationHandler<>).Name && x.ImplementationType?.Name == typeof(DeletedPostNotificationHandler).Name && x.Lifetime == ServiceLifetime.Transient);
        var actUpdatedPartiallyPostNotificationHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(INotificationHandler<>).Name && x.ImplementationType?.Name == typeof(UpdatedPartiallyPostNotificationHandler).Name && x.Lifetime == ServiceLifetime.Transient);
        var actUpdatedPostNotificationHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(INotificationHandler<>).Name && x.ImplementationType?.Name == typeof(UpdatedPostNotificationHandler).Name && x.Lifetime == ServiceLifetime.Transient);

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actMapperServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();

        actCreatePostCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actDeletePostCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatePartialPostCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatePostCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actGetPostByGuidQueryValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actGetPostsQueryValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();

        actCreatePostCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actDeletePostCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatePartialPostCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatePostCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actGetPostByGuidQueryHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actGetPostsQueryHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();

        actCreatedPostNotificationHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actDeletedPostNotificationHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatedPartiallyPostNotificationHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatedPostNotificationHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
    }
}
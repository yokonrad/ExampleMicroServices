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
        var actMapperServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IMapper).Name);
        var actCreatePostCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(CreatePostCommandValidator).Name);
        var actDeletePostCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(DeletePostCommandValidator).Name);
        var actUpdatePartialPostCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(UpdatePartialPostCommandValidator).Name);
        var actUpdatePostCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(UpdatePostCommandValidator).Name);
        var actGetPostByGuidQueryValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(GetPostByGuidQueryValidator).Name);
        var actGetPostsQueryValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(GetPostsQueryValidator).Name);
        var actCreatePostCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(CreatePostCommandHandler).Name);
        var actDeletePostCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(DeletePostCommandHandler).Name);
        var actUpdatePartialPostCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(UpdatePartialPostCommandHandler).Name);
        var actUpdatePostCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(UpdatePostCommandHandler).Name);
        var actGetPostByGuidQueryHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(GetPostByGuidQueryHandler).Name);
        var actGetPostsQueryHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(GetPostsQueryHandler).Name);
        var actCreatedPostNotificationHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(INotificationHandler<>).Name && x.ImplementationType?.Name == typeof(CreatedPostNotificationHandler).Name);
        var actDeletedPostNotificationHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(INotificationHandler<>).Name && x.ImplementationType?.Name == typeof(DeletedPostNotificationHandler).Name);
        var actUpdatedPartiallyPostNotificationHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(INotificationHandler<>).Name && x.ImplementationType?.Name == typeof(UpdatedPartiallyPostNotificationHandler).Name);
        var actUpdatedPostNotificationHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == typeof(INotificationHandler<>).Name && x.ImplementationType?.Name == typeof(UpdatedPostNotificationHandler).Name);

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
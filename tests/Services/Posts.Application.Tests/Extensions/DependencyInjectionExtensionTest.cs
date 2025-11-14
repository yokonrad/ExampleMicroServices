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
        var actMapperServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name == nameof(IMapper));
        var actCreatePostCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IValidator<>)) && x.ImplementationType?.Name == nameof(CreatePostCommandValidator));
        var actDeletePostCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IValidator<>)) && x.ImplementationType?.Name == nameof(DeletePostCommandValidator));
        var actUpdatePartialPostCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IValidator<>)) && x.ImplementationType?.Name == nameof(UpdatePartialPostCommandValidator));
        var actUpdatePostCommandValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IValidator<>)) && x.ImplementationType?.Name == nameof(UpdatePostCommandValidator));
        var actGetPostByGuidQueryValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IValidator<>)) && x.ImplementationType?.Name == nameof(GetPostByGuidQueryValidator));
        var actGetPostQueryValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IValidator<>)) && x.ImplementationType?.Name == nameof(GetPostQueryValidator));
        var actCreatePostCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IRequestHandler<,>)) && x.ImplementationType?.Name == nameof(CreatePostCommandHandler));
        var actDeletePostCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IRequestHandler<,>)) && x.ImplementationType?.Name == nameof(DeletePostCommandHandler));
        var actUpdatePartialPostCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IRequestHandler<,>)) && x.ImplementationType?.Name == nameof(UpdatePartialPostCommandHandler));
        var actUpdatePostCommandHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IRequestHandler<,>)) && x.ImplementationType?.Name == nameof(UpdatePostCommandHandler));
        var actGetPostByGuidQueryHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IRequestHandler<,>)) && x.ImplementationType?.Name == nameof(GetPostByGuidQueryHandler));
        var actGetPostQueryHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IRequestHandler<,>)) && x.ImplementationType?.Name == nameof(GetPostQueryHandler));
        var actCreatedPostNotificationHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(INotificationHandler<>)) && x.ImplementationType?.Name == nameof(CreatedPostNotificationHandler));
        var actDeletedPostNotificationHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(INotificationHandler<>)) && x.ImplementationType?.Name == nameof(DeletedPostNotificationHandler));
        var actUpdatedPartiallyPostNotificationHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(INotificationHandler<>)) && x.ImplementationType?.Name == nameof(UpdatedPartiallyPostNotificationHandler));
        var actUpdatedPostNotificationHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(INotificationHandler<>)) && x.ImplementationType?.Name == nameof(UpdatedPostNotificationHandler));

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actMapperServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actCreatePostCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actDeletePostCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatePartialPostCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatePostCommandValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actGetPostByGuidQueryValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actGetPostQueryValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actCreatePostCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actDeletePostCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatePartialPostCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatePostCommandHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actGetPostByGuidQueryHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actGetPostQueryHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actCreatedPostNotificationHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actDeletedPostNotificationHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatedPartiallyPostNotificationHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actUpdatedPostNotificationHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
    }
}
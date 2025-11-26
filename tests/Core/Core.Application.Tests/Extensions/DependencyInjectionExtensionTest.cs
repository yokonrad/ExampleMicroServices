using Core.Application.Behaviors;
using Core.Application.Extensions;
using Core.Application.Tests.Examples;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Tests.Extensions;

public class DependencyInjectionExtensionTest
{
    [Test]
    public void AddFluentValidationSupport_IServiceCollection_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFluentValidationSupport();

        // Act
        var act = serviceCollection.BuildServiceProvider();
        var actExampleRequestValidatorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IValidator<>).Name && x.ImplementationType?.Name == typeof(ExampleRequestValidator).Name && x.Lifetime == ServiceLifetime.Scoped);

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actExampleRequestValidatorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
    }

    [Test]
    public void AddFluentValidationSupport_ValidatorOptions_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFluentValidationSupport();

        // Act
        var actValidatorOptionsGlobalLanguageManager = ValidatorOptions.Global.LanguageManager.Enabled;
        var actValidatorOptionsGlobalDisplayNameResolver = ValidatorOptions.Global.DisplayNameResolver;
        var actValidatorOptionsGlobalPropertyNameResolver = ValidatorOptions.Global.PropertyNameResolver;

        // Assert
        actValidatorOptionsGlobalLanguageManager.Should().BeFalse();
        actValidatorOptionsGlobalDisplayNameResolver.Should().NotBeNull();
        actValidatorOptionsGlobalPropertyNameResolver.Should().NotBeNull();
    }

    [Test]
    public void AddFluentValidationSupport_ValidatorOptions_DisplayNameResolver_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFluentValidationSupport();

        // Act
        var actValidatorOptionsGlobalDisplayNameResolver = ValidatorOptions.Global.DisplayNameResolver(typeof(ExampleRequest), typeof(ExampleRequest).GetProperty(nameof(ExampleRequest.Example)), null);

        // Assert
        actValidatorOptionsGlobalDisplayNameResolver.Should().Be(nameof(ExampleRequest.Example).ToCamelCase());
    }

    [Test]
    public void AddFluentValidationSupport_ValidatorOptions_PropertyNameResolver_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFluentValidationSupport();

        // Act
        var actValidatorOptionsGlobalPropertyNameResolver = ValidatorOptions.Global.PropertyNameResolver(typeof(ExampleRequest), typeof(ExampleRequest).GetProperty(nameof(ExampleRequest.Example)), null);

        // Assert
        actValidatorOptionsGlobalPropertyNameResolver.Should().Be(nameof(ExampleRequest.Example).ToCamelCase());
    }

    [Test]
    public void AddMediatRSupport_IServiceCollection_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediatRSupport();

        // Act
        var act = serviceCollection.BuildServiceProvider();
        var actExampleRequestHandlerServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IRequestHandler<,>).Name && x.ImplementationType?.Name == typeof(ExampleRequestHandler).Name && x.Lifetime == ServiceLifetime.Transient);

        var actExceptionBehaviorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IPipelineBehavior<,>).Name && x.ImplementationType?.Name == typeof(ExceptionBehavior<,>).Name && x.Lifetime == ServiceLifetime.Transient);
        var actLoggingBehaviorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IPipelineBehavior<,>).Name && x.ImplementationType?.Name == typeof(LoggingBehavior<,>).Name && x.Lifetime == ServiceLifetime.Transient);
        var actPerformanceBehaviorServiceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType.Name == typeof(IPipelineBehavior<,>).Name && x.ImplementationType?.Name == typeof(PerformanceBehavior<,>).Name && x.Lifetime == ServiceLifetime.Transient);

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actExampleRequestHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();

        actExceptionBehaviorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actLoggingBehaviorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actPerformanceBehaviorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
    }
}
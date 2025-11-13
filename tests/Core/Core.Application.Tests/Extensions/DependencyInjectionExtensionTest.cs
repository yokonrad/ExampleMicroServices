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
        var actExampleRequestValidatorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IValidator<>)) && x.ImplementationType?.Name == nameof(ExampleRequestValidator));

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
        var actValidatorOptionsLanguageManager = ValidatorOptions.Global.LanguageManager.Enabled;
        var actValidatorOptionsDisplayNameResolver = ValidatorOptions.Global.DisplayNameResolver;
        var actValidatorOptionsPropertyNameResolver = ValidatorOptions.Global.PropertyNameResolver;

        // Assert
        actValidatorOptionsLanguageManager.Should().BeFalse();
        actValidatorOptionsDisplayNameResolver.Should().NotBeNull();
        actValidatorOptionsPropertyNameResolver.Should().NotBeNull();
    }

    [Test]
    public void AddFluentValidationSupport_ValidatorOptions_DisplayNameResolver_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFluentValidationSupport();

        // Act
        var actValidatorOptionsDisplayNameResolver = ValidatorOptions.Global.DisplayNameResolver(typeof(ExampleRequest), typeof(ExampleRequest).GetProperty(nameof(ExampleRequest.Example)), null);

        // Assert
        actValidatorOptionsDisplayNameResolver.Should().Be(nameof(ExampleRequest.Example).ToCamelCase());
    }

    [Test]
    public void AddFluentValidationSupport_ValidatorOptions_PropertyNameResolver_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFluentValidationSupport();

        // Act
        var actValidatorOptionsPropertyNameResolver = ValidatorOptions.Global.PropertyNameResolver(typeof(ExampleRequest), typeof(ExampleRequest).GetProperty(nameof(ExampleRequest.Example)), null);

        // Assert
        actValidatorOptionsPropertyNameResolver.Should().Be(nameof(ExampleRequest.Example).ToCamelCase());
    }

    [Test]
    public void AddMediatRSupport_IServiceCollection_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediatRSupport();

        // Act
        var act = serviceCollection.BuildServiceProvider();
        var actExampleRequestHandlerServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IRequestHandler<,>)) && x.ImplementationType?.Name == nameof(ExampleRequestHandler));
        var actExceptionBehaviorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IPipelineBehavior<,>)) && x.ImplementationType?.Name.Contains(nameof(ExceptionBehavior<,>)) == true);
        var actLoggingBehaviorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IPipelineBehavior<,>)) && x.ImplementationType?.Name.Contains(nameof(LoggingBehavior<,>)) == true);
        var actPerformanceBehaviorServiceDescriptor = serviceCollection.First(x => x.ServiceType.Name.Contains(nameof(IPipelineBehavior<,>)) && x.ImplementationType?.Name.Contains(nameof(PerformanceBehavior<,>)) == true);

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actExampleRequestHandlerServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actExceptionBehaviorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actLoggingBehaviorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
        actPerformanceBehaviorServiceDescriptor.Should().BeOfType<ServiceDescriptor>().And.NotBeNull();
    }
}
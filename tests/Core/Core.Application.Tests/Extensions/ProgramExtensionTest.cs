using Core.Application.Extensions;
using Core.Application.Tests.Examples;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Tests.Extensions;

public class ProgramExtensionTest
{
    [Test]
    public void AddFluentValidationSupport_IServiceCollection_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFluentValidationSupport();

        // Act
        var act = serviceCollection.BuildServiceProvider();
        var actValidators = serviceCollection.Where(x => x.ServiceType.Name.Contains(nameof(IValidator))).ToArray();
        var actExampleRequestValidator = act.GetRequiredService<IValidator<ExampleRequest>>();

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actValidators.Should().BeOfType<ServiceDescriptor[]>().And.NotBeNull().And.HaveCount(1);
        actExampleRequestValidator.Should().BeOfType<ExampleRequestValidator>().And.NotBeNull();
    }

    [Test]
    public void AddMediatRSupport_IServiceCollection_Should_Be_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediatRSupport();

        // Act
        var act = serviceCollection.BuildServiceProvider();
        var actRequestHandlers = serviceCollection.Where(x => x.ServiceType.Name.Contains(nameof(IRequestHandler<,>))).ToArray();
        var actPipelineBehaviors = serviceCollection.Where(x => x.ServiceType.Name.Contains(nameof(IPipelineBehavior<,>))).ToArray();

        // Assert
        act.Should().BeOfType<ServiceProvider>().And.NotBeNull();
        actRequestHandlers.Should().BeOfType<ServiceDescriptor[]>().And.NotBeNull().And.HaveCount(1);
        actPipelineBehaviors.Should().BeOfType<ServiceDescriptor[]>().And.NotBeNull().And.HaveCount(3);
    }
}
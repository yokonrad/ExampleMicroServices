using AutoBogus;
using Core.Application.Extensions;
using Core.Application.Tests.Examples;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Core.Application.Tests.Extensions;

public class FluentValidationExtensionTest
{
    [Test]
    public void GetErrors_Should_Be_Not_Empty_When_Invalid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddValidatorsFromAssemblyContaining<ExampleRequestValidator>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var exampleRequestValidator = serviceProvider.GetRequiredService<IValidator<ExampleRequest>>();

        var fakerExampleRequest = new AutoFaker<ExampleRequest>()
            .RuleFor(x => x.Example, _ => It.IsAny<string>());

        var exampleRequest = fakerExampleRequest.Generate();

        // Act
        var act = exampleRequestValidator.TestValidate(exampleRequest).GetValidationErrors();

        // Assert
        act.Should().BeOfType<IError[]>().And.NotBeNull().And.NotBeEmpty();
    }

    [Test]
    public void GetErrors_Should_Be_Empty_When_Valid()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddValidatorsFromAssemblyContaining<ExampleRequestValidator>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var exampleRequestValidator = serviceProvider.GetRequiredService<IValidator<ExampleRequest>>();

        var fakerExampleRequest = new AutoFaker<ExampleRequest>();
        var exampleRequest = fakerExampleRequest.Generate();

        // Act
        var act = exampleRequestValidator.TestValidate(exampleRequest).GetValidationErrors();

        // Assert
        act.Should().BeOfType<IError[]>().And.NotBeNull().And.BeEmpty();
    }
}
using AutoBogus;
using Core.Application.Errors;
using FluentAssertions;

namespace Core.Application.Tests.Errors;

public class ValidationErrorTest
{
    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var fakerValidationError = new AutoFaker<ValidationError>();

        // Act
        var act = fakerValidationError.Generate();

        // Assert
        act.Should().BeOfType<ValidationError>().And.NotBeNull();
    }
}
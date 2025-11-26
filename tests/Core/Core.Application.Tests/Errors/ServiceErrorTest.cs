using AutoBogus;
using Core.Application.Errors;
using FluentAssertions;

namespace Core.Application.Tests.Errors;

public class ServiceErrorTest
{
    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var fakerServiceError = new AutoFaker<ServiceError>();

        // Act
        var act = fakerServiceError.Generate();

        // Assert
        act.Should().BeOfType<ServiceError>().And.NotBeNull();
    }
}
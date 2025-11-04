using AutoBogus;
using Core.Application.Errors;
using FluentAssertions;

namespace Core.Application.Tests.Errors;

public class NotFoundErrorTest
{
    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var fakerNotFoundError = new AutoFaker<NotFoundError>();

        // Act
        var act = fakerNotFoundError.Generate();

        // Assert
        act.Should().BeOfType<NotFoundError>().And.NotBeNull();
    }
}
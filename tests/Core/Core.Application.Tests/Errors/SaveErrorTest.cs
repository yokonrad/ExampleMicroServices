using AutoBogus;
using Core.Application.Errors;
using FluentAssertions;

namespace Core.Application.Tests.Errors;

public class SaveErrorTest
{
    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var fakerSaveError = new AutoFaker<SaveError>();

        // Act
        var act = fakerSaveError.Generate();

        // Assert
        act.Should().BeOfType<SaveError>().And.NotBeNull();
    }
}
using Core.Application.Errors;
using Core.Application.Extensions;
using FluentAssertions;
using FluentResults;
using FluentResults.Extensions.FluentAssertions;

namespace Core.Application.Tests.Extensions;

public class FluentResultsExtensionTest
{
    [Test]
    public void GetValidationErrors_Should_Be_Not_Empty_When_Fail()
    {
        // Arrange
        var result = Result.Fail<ValidationError>([new ValidationError("Property name", "Error message")]);

        // Act
        var act = result.GetValidationErrors();

        // Assert
        act.Should().BeOfType<Dictionary<string, string[]>>().And.NotBeNull().And.NotBeEmpty();
    }

    [Test]
    public void GetValidationErrors_Should_Be_Empty_When_Ok()
    {
        // Arrange
        var result = Result.Ok("Success message");

        // Act
        var act = result.GetValidationErrors();

        // Assert
        act.Should().BeOfType<Dictionary<string, string[]>>().And.NotBeNull().And.BeEmpty();
    }
}
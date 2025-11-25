using Core.Application.Extensions;
using FluentAssertions;

namespace Core.Application.Tests.Extensions;

public class StringExtensionTest
{
    [Test]
    public void ToCamelCase_Should_Be_Valid()
    {
        // Arrange
        const string input = "ExampleString";
        const string output = "exampleString";

        // Act
        var act = input.ToCamelCase();

        // Assert
        act.Should().Be(output);
    }
}
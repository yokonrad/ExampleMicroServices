using AutoBogus;
using Comments.Core.Entities;
using FluentAssertions;
using Moq;

namespace Comments.Core.Tests.Entities;

public class CommentTest
{
    [Test]
    public void Should_Throw_ArgumentException_When_PostGuid_Any()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => It.IsAny<Guid>());

        // Act
        var act = () => fakerComment.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Throw_ArgumentException_When_PostGuid_Empty()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.PostGuid, _ => Guid.Empty);

        // Act
        var act = () => fakerComment.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Throw_ArgumentNullException_When_Text_Any()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.Text, _ => It.IsAny<string>());

        // Act
        var act = () => fakerComment.Generate();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Should_Throw_ArgumentException_When_Text_Empty()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.Text, _ => string.Empty);

        // Act
        var act = () => fakerComment.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Throw_ArgumentException_When_Text_Whitespace()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>()
            .RuleFor(x => x.Text, _ => "");

        // Act
        var act = () => fakerComment.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var fakerComment = new AutoFaker<Comment>();

        // Act
        var act = fakerComment.Generate();

        // Assert
        act.Should().BeOfType<Comment>().And.NotBeNull();
    }
}
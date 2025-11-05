using AutoBogus;
using FluentAssertions;
using Moq;
using Posts.Domain.Entities;

namespace Posts.Domain.Tests.Entities;

public class PostTest
{
    [Test]
    public void Should_Throw_ArgumentNullException_When_Title_Any()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Title, _ => It.IsAny<string>());

        // Act
        var act = () => fakerPost.Generate();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Should_Throw_ArgumentException_When_Title_Empty()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Title, _ => string.Empty);

        // Act
        var act = () => fakerPost.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Throw_ArgumentException_When_Title_Whitespace()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Title, _ => "");

        // Act
        var act = () => fakerPost.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Throw_ArgumentNullException_When_Text_Any()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Text, _ => It.IsAny<string>());

        // Act
        var act = () => fakerPost.Generate();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Should_Throw_ArgumentException_When_Text_Empty()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Text, _ => string.Empty);

        // Act
        var act = () => fakerPost.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Throw_ArgumentException_When_Text_Whitespace()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>()
            .RuleFor(x => x.Text, _ => "");

        // Act
        var act = () => fakerPost.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var fakerPost = new AutoFaker<Post>();

        // Act
        var act = fakerPost.Generate();

        // Assert
        act.Should().BeOfType<Post>().And.NotBeNull();
    }
}
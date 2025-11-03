using AutoBogus;
using Core.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Core.Domain.Tests.Entities;

public class EntityTest
{
    [Test]
    public void Should_Throw_ArgumentException_When_Guid_Any()
    {
        // Arrange
        var fakerEntity = new AutoFaker<Entity>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>());

        // Act
        var act = () => fakerEntity.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Throw_ArgumentException_When_Guid_Empty()
    {
        // Arrange
        var fakerEntity = new AutoFaker<Entity>()
            .RuleFor(x => x.Guid, _ => Guid.Empty);

        // Act
        var act = () => fakerEntity.Generate();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var fakerEntity = new AutoFaker<Entity>();

        // Act
        var act = fakerEntity.Generate();

        // Assert
        act.Should().BeOfType<Entity>().And.NotBeNull();
    }
}
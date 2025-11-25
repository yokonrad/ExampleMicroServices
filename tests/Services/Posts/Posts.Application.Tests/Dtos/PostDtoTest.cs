using AutoBogus;
using FluentAssertions;
using Posts.Application.Dtos;

namespace Posts.Application.Tests.Dtos;

public class PostDtoTest
{
    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();

        // Act
        var act = fakerPostDto.Generate();

        // Assert
        act.Should().BeOfType<PostDto>().And.NotBeNull();
    }
}
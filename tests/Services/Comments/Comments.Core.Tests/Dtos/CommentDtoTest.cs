using AutoBogus;
using Comments.Core.Dtos;
using FluentAssertions;

namespace Comments.Core.Tests.Dtos;

public class CommentDtoTest
{
    [Test]
    public void Should_Be_Valid()
    {
        // Arrange
        var fakerCommentDto = new AutoFaker<CommentDto>();

        // Act
        var act = fakerCommentDto.Generate();

        // Assert
        act.Should().BeOfType<CommentDto>().And.NotBeNull();
    }
}
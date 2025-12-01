using AutoBogus;
using Comments.Core.Commands;
using Comments.Core.Dtos;
using Comments.WebAPI.Endpoints;
using Core.Application.Errors;
using FastEndpoints;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Comments.WebAPI.Tests.Endpoints;

public class PostCommentEndpointTest
{
    private Mock<IMediator> mockMediator;

    [SetUp]
    public void SetUp()
    {
        mockMediator = new Mock<IMediator>();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_ValidationError()
    {
        // Arrange
        var commentCommentRequest = new AutoFaker<PostCommentRequest>()
            .RuleFor(x => x.PostGuid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>())
            .Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<CreateCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var commentCommentEndpoint = Factory.Create<PostCommentEndpoint>(mockMediator.Object);

        // Act
        await commentCommentEndpoint.HandleAsync(commentCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        commentCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        commentCommentEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_ServiceError()
    {
        // Arrange
        var commentCommentRequest = new AutoFaker<PostCommentRequest>()
            .Generate();

        var result = Result.Fail(new ServiceError());

        mockMediator.Setup(x => x.Send(It.IsAny<CreateCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var commentCommentEndpoint = Factory.Create<PostCommentEndpoint>(mockMediator.Object);

        // Act
        await commentCommentEndpoint.HandleAsync(commentCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        commentCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        commentCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_SaveError()
    {
        // Arrange
        var commentCommentRequest = new AutoFaker<PostCommentRequest>()
            .Generate();

        var result = Result.Fail(new SaveError());

        mockMediator.Setup(x => x.Send(It.IsAny<CreateCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var commentCommentEndpoint = Factory.Create<PostCommentEndpoint>(mockMediator.Object);

        // Act
        await commentCommentEndpoint.HandleAsync(commentCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        commentCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        commentCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var commentDto = new AutoFaker<CommentDto>()
            .Generate();

        var postCommentRequest = new AutoFaker<PostCommentRequest>()
            .RuleFor(x => x.PostGuid, _ => commentDto.PostGuid)
            .RuleFor(x => x.Text, _ => commentDto.Text)
            .RuleFor(x => x.Visible, _ => commentDto.Visible)
            .Generate();

        var postCommentResponse = new PostCommentMapper()
            .FromEntity(commentDto);

        var result = Result.Ok(commentDto);

        mockMediator.Setup(x => x.Send(It.IsAny<CreateCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var postCommentEndpoint = Factory.Create<PostCommentEndpoint>(mockMediator.Object);

        // Act
        await postCommentEndpoint.HandleAsync(postCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        postCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        postCommentEndpoint.ValidationFailed.Should().BeFalse();
        postCommentEndpoint.Response.Should().BeAssignableTo<PostCommentResponse>().And.BeEquivalentTo(postCommentResponse).And.NotBeNull();
    }
}
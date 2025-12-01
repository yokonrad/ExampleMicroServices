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

public class PutCommentEndpointTest
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
        var putCommentRequest = new AutoFaker<PutCommentRequest>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.PostGuid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>())
            .Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<UpdateCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var putCommentEndpoint = Factory.Create<PutCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PutCommentRequest.Guid), putCommentRequest.Guid), mockMediator.Object);

        // Act
        await putCommentEndpoint.HandleAsync(putCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        putCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        putCommentEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_NotFoundError()
    {
        // Arrange
        var putCommentRequest = new AutoFaker<PutCommentRequest>()
            .Generate();

        var result = Result.Fail(new NotFoundError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdateCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var putCommentEndpoint = Factory.Create<PutCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PutCommentRequest.Guid), putCommentRequest.Guid), mockMediator.Object);

        // Act
        await putCommentEndpoint.HandleAsync(putCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        putCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        putCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_ServiceError()
    {
        // Arrange
        var putCommentRequest = new AutoFaker<PutCommentRequest>()
            .Generate();

        var result = Result.Fail(new ServiceError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdateCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var putCommentEndpoint = Factory.Create<PutCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PutCommentRequest.Guid), putCommentRequest.Guid), mockMediator.Object);

        // Act
        await putCommentEndpoint.HandleAsync(putCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        putCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        putCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_SaveError()
    {
        // Arrange
        var putCommentRequest = new AutoFaker<PutCommentRequest>()
            .Generate();

        var result = Result.Fail(new SaveError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdateCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var putCommentEndpoint = Factory.Create<PutCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PutCommentRequest.Guid), putCommentRequest.Guid), mockMediator.Object);

        // Act
        await putCommentEndpoint.HandleAsync(putCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        putCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        putCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var commentDto = new AutoFaker<CommentDto>()
            .Generate();

        var putCommentRequest = new AutoFaker<PutCommentRequest>()
            .RuleFor(x => x.PostGuid, _ => commentDto.PostGuid)
            .RuleFor(x => x.Text, _ => commentDto.Text)
            .RuleFor(x => x.Visible, _ => commentDto.Visible)
            .Generate();

        var putCommentResponse = new PutCommentMapper()
            .FromEntity(commentDto);

        var result = Result.Ok(commentDto);

        mockMediator.Setup(x => x.Send(It.IsAny<UpdateCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var putCommentEndpoint = Factory.Create<PutCommentEndpoint>(mockMediator.Object);

        // Act
        await putCommentEndpoint.HandleAsync(putCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        putCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        putCommentEndpoint.ValidationFailed.Should().BeFalse();
        putCommentEndpoint.Response.Should().BeAssignableTo<PutCommentResponse>().And.BeEquivalentTo(putCommentResponse).And.NotBeNull();
    }
}
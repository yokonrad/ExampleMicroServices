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

public class DeleteCommentEndpointTest
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
        var deleteCommentRequest = new AutoFaker<DeleteCommentRequest>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<DeleteCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var deleteCommentEndpoint = Factory.Create<DeleteCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(DeleteCommentRequest.Guid), deleteCommentRequest.Guid), mockMediator.Object);

        // Act
        await deleteCommentEndpoint.HandleAsync(deleteCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        deleteCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        deleteCommentEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_NotFoundError()
    {
        // Arrange
        var deleteCommentRequest = new AutoFaker<DeleteCommentRequest>()
            .Generate();

        var result = Result.Fail(new NotFoundError());

        mockMediator.Setup(x => x.Send(It.IsAny<DeleteCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var deleteCommentEndpoint = Factory.Create<DeleteCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(DeleteCommentRequest.Guid), deleteCommentRequest.Guid), mockMediator.Object);

        // Act
        await deleteCommentEndpoint.HandleAsync(deleteCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        deleteCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        deleteCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_ServiceError()
    {
        // Arrange
        var deleteCommentRequest = new AutoFaker<DeleteCommentRequest>()
            .Generate();

        var result = Result.Fail(new ServiceError());

        mockMediator.Setup(x => x.Send(It.IsAny<DeleteCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var deleteCommentEndpoint = Factory.Create<DeleteCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(DeleteCommentRequest.Guid), deleteCommentRequest.Guid), mockMediator.Object);

        // Act
        await deleteCommentEndpoint.HandleAsync(deleteCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        deleteCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        deleteCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_SaveError()
    {
        // Arrange
        var deleteCommentRequest = new AutoFaker<DeleteCommentRequest>()
            .Generate();

        var result = Result.Fail(new SaveError());

        mockMediator.Setup(x => x.Send(It.IsAny<DeleteCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var deleteCommentEndpoint = Factory.Create<DeleteCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(DeleteCommentRequest.Guid), deleteCommentRequest.Guid), mockMediator.Object);

        // Act
        await deleteCommentEndpoint.HandleAsync(deleteCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        deleteCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        deleteCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var commentDto = new AutoFaker<CommentDto>()
            .Generate();

        var deleteCommentRequest = new AutoFaker<DeleteCommentRequest>()
            .RuleFor(x => x.Guid, _ => commentDto.Guid)
            .Generate();

        var deleteCommentResponse = new DeleteCommentMapper()
            .FromEntity(commentDto);

        var result = Result.Ok(commentDto);

        mockMediator.Setup(x => x.Send(It.IsAny<DeleteCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var deleteCommentEndpoint = Factory.Create<DeleteCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(DeleteCommentRequest.Guid), deleteCommentRequest.Guid), mockMediator.Object);

        // Act
        await deleteCommentEndpoint.HandleAsync(deleteCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        deleteCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        deleteCommentEndpoint.ValidationFailed.Should().BeFalse();
        deleteCommentEndpoint.Response.Should().BeAssignableTo<DeleteCommentResponse>().And.BeEquivalentTo(deleteCommentResponse).And.NotBeNull();
    }
}
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

public class PatchCommentEndpointTest
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
        var patchCommentRequest = new AutoFaker<PatchCommentRequest>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.PostGuid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>())
            .Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePartialCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var patchCommentEndpoint = Factory.Create<PatchCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PatchCommentRequest.Guid), patchCommentRequest.Guid), mockMediator.Object);

        // Act
        await patchCommentEndpoint.HandleAsync(patchCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        patchCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        patchCommentEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_NotFoundError()
    {
        // Arrange
        var patchCommentRequest = new AutoFaker<PatchCommentRequest>()
            .Generate();

        var result = Result.Fail(new NotFoundError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePartialCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var patchCommentEndpoint = Factory.Create<PatchCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PatchCommentRequest.Guid), patchCommentRequest.Guid), mockMediator.Object);

        // Act
        await patchCommentEndpoint.HandleAsync(patchCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        patchCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        patchCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_ServiceError()
    {
        // Arrange
        var patchCommentRequest = new AutoFaker<PatchCommentRequest>()
            .Generate();

        var result = Result.Fail(new ServiceError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePartialCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var patchCommentEndpoint = Factory.Create<PatchCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PatchCommentRequest.Guid), patchCommentRequest.Guid), mockMediator.Object);

        // Act
        await patchCommentEndpoint.HandleAsync(patchCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        patchCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        patchCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_SaveError()
    {
        // Arrange
        var patchCommentRequest = new AutoFaker<PatchCommentRequest>()
            .Generate();

        var result = Result.Fail(new SaveError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePartialCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var patchCommentEndpoint = Factory.Create<PatchCommentEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PatchCommentRequest.Guid), patchCommentRequest.Guid), mockMediator.Object);

        // Act
        await patchCommentEndpoint.HandleAsync(patchCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        patchCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        patchCommentEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var commentDto = new AutoFaker<CommentDto>()
            .Generate();

        var patchCommentRequest = new AutoFaker<PatchCommentRequest>()
            .RuleFor(x => x.Guid, _ => commentDto.Guid)
            .Generate();

        var patchCommentResponse = new PatchCommentMapper()
            .FromEntity(commentDto);

        var result = Result.Ok(commentDto);

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePartialCommentCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var patchCommentEndpoint = Factory.Create<PatchCommentEndpoint>(mockMediator.Object);

        // Act
        await patchCommentEndpoint.HandleAsync(patchCommentRequest, It.IsAny<CancellationToken>());

        // Assert
        patchCommentEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        patchCommentEndpoint.ValidationFailed.Should().BeFalse();
        patchCommentEndpoint.Response.Should().BeAssignableTo<PatchCommentResponse>().And.BeEquivalentTo(patchCommentResponse).And.NotBeNull();
    }
}
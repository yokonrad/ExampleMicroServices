using AutoBogus;
using Core.Application.Errors;
using FastEndpoints;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Posts.Application.Commands;
using Posts.Application.Dtos;
using Posts.WebAPI.Endpoints;

namespace Posts.WebAPI.Tests.Endpoints;

public class DeletePostEndpointTest
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
        var deletePostRequest = new AutoFaker<DeletePostRequest>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<DeletePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var deletePostEndpoint = Factory.Create<DeletePostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(DeletePostRequest.Guid), deletePostRequest.Guid), mockMediator.Object);

        // Act
        await deletePostEndpoint.HandleAsync(deletePostRequest, It.IsAny<CancellationToken>());

        // Assert
        deletePostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        deletePostEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_NotFoundError()
    {
        // Arrange
        var deletePostRequest = new AutoFaker<DeletePostRequest>()
            .Generate();

        var result = Result.Fail(new NotFoundError());

        mockMediator.Setup(x => x.Send(It.IsAny<DeletePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var deletePostEndpoint = Factory.Create<DeletePostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(DeletePostRequest.Guid), deletePostRequest.Guid), mockMediator.Object);

        // Act
        await deletePostEndpoint.HandleAsync(deletePostRequest, It.IsAny<CancellationToken>());

        // Assert
        deletePostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        deletePostEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_SaveError()
    {
        // Arrange
        var deletePostRequest = new AutoFaker<DeletePostRequest>()
            .Generate();

        var result = Result.Fail(new SaveError());

        mockMediator.Setup(x => x.Send(It.IsAny<DeletePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var deletePostEndpoint = Factory.Create<DeletePostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(DeletePostRequest.Guid), deletePostRequest.Guid), mockMediator.Object);

        // Act
        await deletePostEndpoint.HandleAsync(deletePostRequest, It.IsAny<CancellationToken>());

        // Assert
        deletePostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        deletePostEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var postDto = new AutoFaker<PostDto>()
            .Generate();

        var deletePostRequest = new AutoFaker<DeletePostRequest>()
            .RuleFor(x => x.Guid, _ => postDto.Guid)
            .Generate();

        var deletePostResponse = new DeletePostMapper()
            .FromEntity(postDto);

        var result = Result.Ok(postDto);

        mockMediator.Setup(x => x.Send(It.IsAny<DeletePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var deletePostEndpoint = Factory.Create<DeletePostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(DeletePostRequest.Guid), deletePostRequest.Guid), mockMediator.Object);

        // Act
        await deletePostEndpoint.HandleAsync(deletePostRequest, It.IsAny<CancellationToken>());

        // Assert
        deletePostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        deletePostEndpoint.ValidationFailed.Should().BeFalse();
        deletePostEndpoint.Response.Should().BeAssignableTo<DeletePostResponse>().And.BeEquivalentTo(deletePostResponse).And.NotBeNull();
    }
}
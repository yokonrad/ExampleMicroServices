using AutoBogus;
using Comments.Core.Dtos;
using Comments.Core.Queries;
using Comments.WebAPI.Endpoints;
using Core.Application.Errors;
using FastEndpoints;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Comments.WebAPI.Tests.Endpoints;

public class GetCommentByGuidEndpointTest
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
        var getCommentByGuidRequest = new AutoFaker<GetCommentByGuidRequest>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<GetCommentByGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getCommentByGuidEndpoint = Factory.Create<GetCommentByGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetCommentByGuidRequest.Guid), getCommentByGuidRequest.Guid), mockMediator.Object);

        // Act
        await getCommentByGuidEndpoint.HandleAsync(getCommentByGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getCommentByGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        getCommentByGuidEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_NotFoundError()
    {
        // Arrange
        var getCommentByGuidRequest = new AutoFaker<GetCommentByGuidRequest>()
            .Generate();

        var result = Result.Fail(new NotFoundError());

        mockMediator.Setup(x => x.Send(It.IsAny<GetCommentByGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getCommentByGuidEndpoint = Factory.Create<GetCommentByGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetCommentByGuidRequest.Guid), getCommentByGuidRequest.Guid), mockMediator.Object);

        // Act
        await getCommentByGuidEndpoint.HandleAsync(getCommentByGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getCommentByGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        getCommentByGuidEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_ServiceError()
    {
        // Arrange
        var getCommentByGuidRequest = new AutoFaker<GetCommentByGuidRequest>()
            .Generate();

        var result = Result.Fail(new ServiceError());

        mockMediator.Setup(x => x.Send(It.IsAny<GetCommentByGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getCommentByGuidEndpoint = Factory.Create<GetCommentByGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetCommentByGuidRequest.Guid), getCommentByGuidRequest.Guid), mockMediator.Object);

        // Act
        await getCommentByGuidEndpoint.HandleAsync(getCommentByGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getCommentByGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        getCommentByGuidEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var commentDto = new AutoFaker<CommentDto>()
            .Generate();

        var getCommentByGuidRequest = new AutoFaker<GetCommentByGuidRequest>()
            .RuleFor(x => x.Guid, _ => commentDto.Guid)
            .Generate();

        var getCommentByGuidResponse = new GetCommentByGuidMapper()
            .FromEntity(commentDto);

        var result = Result.Ok(commentDto);

        mockMediator.Setup(x => x.Send(It.IsAny<GetCommentByGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getCommentByGuidEndpoint = Factory.Create<GetCommentByGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetCommentByGuidRequest.Guid), getCommentByGuidRequest.Guid), mockMediator.Object);

        // Act
        await getCommentByGuidEndpoint.HandleAsync(getCommentByGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getCommentByGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        getCommentByGuidEndpoint.ValidationFailed.Should().BeFalse();
        getCommentByGuidEndpoint.Response.Should().BeAssignableTo<GetCommentByGuidResponse>().And.BeEquivalentTo(getCommentByGuidResponse).And.NotBeNull();
    }
}
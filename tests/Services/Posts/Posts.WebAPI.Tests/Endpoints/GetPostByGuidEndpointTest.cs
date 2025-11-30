using AutoBogus;
using Core.Application.Errors;
using FastEndpoints;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Posts.Application.Dtos;
using Posts.Application.Queries;
using Posts.WebAPI.Endpoints;

namespace Posts.WebAPI.Tests.Endpoints;

public class GetPostByGuidEndpointTest
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
        var getPostByGuidRequest = new AutoFaker<GetPostByGuidRequest>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<GetPostByGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getPostByGuidEndpoint = Factory.Create<GetPostByGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetPostByGuidRequest.Guid), getPostByGuidRequest.Guid), mockMediator.Object);

        // Act
        await getPostByGuidEndpoint.HandleAsync(getPostByGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getPostByGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        getPostByGuidEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_NotFoundError()
    {
        // Arrange
        var getPostByGuidRequest = new AutoFaker<GetPostByGuidRequest>()
            .Generate();

        var result = Result.Fail(new NotFoundError());

        mockMediator.Setup(x => x.Send(It.IsAny<GetPostByGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getPostByGuidEndpoint = Factory.Create<GetPostByGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetPostByGuidRequest.Guid), getPostByGuidRequest.Guid), mockMediator.Object);

        // Act
        await getPostByGuidEndpoint.HandleAsync(getPostByGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getPostByGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        getPostByGuidEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var postDto = new AutoFaker<PostDto>()
            .Generate();

        var getPostByGuidRequest = new AutoFaker<GetPostByGuidRequest>()
            .RuleFor(x => x.Guid, _ => postDto.Guid)
            .Generate();

        var getPostByGuidResponse = new GetPostByGuidMapper()
            .FromEntity(postDto);

        var result = Result.Ok(postDto);

        mockMediator.Setup(x => x.Send(It.IsAny<GetPostByGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getPostByGuidEndpoint = Factory.Create<GetPostByGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetPostByGuidRequest.Guid), getPostByGuidRequest.Guid), mockMediator.Object);

        // Act
        await getPostByGuidEndpoint.HandleAsync(getPostByGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getPostByGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        getPostByGuidEndpoint.ValidationFailed.Should().BeFalse();
        getPostByGuidEndpoint.Response.Should().BeAssignableTo<GetPostByGuidResponse>().And.BeEquivalentTo(getPostByGuidResponse).And.NotBeNull();
    }
}
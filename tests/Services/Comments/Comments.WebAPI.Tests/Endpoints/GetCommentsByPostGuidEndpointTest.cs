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

public class GetCommentsByPostGuidEndpointTest
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
        var getCommentsByPostGuidRequest = new AutoFaker<GetCommentsByPostGuidRequest>()
            .RuleFor(x => x.PostGuid, _ => It.IsAny<Guid>())
            .Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<GetCommentsByPostGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getCommentsByPostGuidEndpoint = Factory.Create<GetCommentsByPostGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetCommentsByPostGuidRequest.PostGuid), getCommentsByPostGuidRequest.PostGuid), mockMediator.Object);

        // Act
        await getCommentsByPostGuidEndpoint.HandleAsync(getCommentsByPostGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getCommentsByPostGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        getCommentsByPostGuidEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_ServiceError()
    {
        // Arrange
        var getCommentsByPostGuidRequest = new AutoFaker<GetCommentsByPostGuidRequest>()
            .Generate();

        var result = Result.Fail(new ServiceError());

        mockMediator.Setup(x => x.Send(It.IsAny<GetCommentsByPostGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getCommentsByPostGuidEndpoint = Factory.Create<GetCommentsByPostGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetCommentsByPostGuidRequest.PostGuid), getCommentsByPostGuidRequest.PostGuid), mockMediator.Object);

        // Act
        await getCommentsByPostGuidEndpoint.HandleAsync(getCommentsByPostGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getCommentsByPostGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        getCommentsByPostGuidEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid_When_Result_Empty()
    {
        // Arrange
        var commentDtos = Enumerable.Empty<CommentDto>();

        var getCommentsByPostGuidRequest = new AutoFaker<GetCommentsByPostGuidRequest>()
            .Generate();

        var getCommentsByPostGuidResponse = new GetCommentsByPostGuidMapper()
            .FromEntity(commentDtos);

        var result = Result.Ok(commentDtos);

        mockMediator.Setup(x => x.Send(It.IsAny<GetCommentsByPostGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getCommentsByPostGuidEndpoint = Factory.Create<GetCommentsByPostGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetCommentsByPostGuidRequest.PostGuid), getCommentsByPostGuidRequest.PostGuid), mockMediator.Object);

        // Act
        await getCommentsByPostGuidEndpoint.HandleAsync(getCommentsByPostGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getCommentsByPostGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        getCommentsByPostGuidEndpoint.ValidationFailed.Should().BeFalse();
        getCommentsByPostGuidEndpoint.Response.Should().BeAssignableTo<IEnumerable<GetCommentsByPostGuidResponse>>().And.BeEquivalentTo(getCommentsByPostGuidResponse).And.BeEmpty();
    }

    [Test]
    public async Task Should_Be_Valid_When_Result_Not_Empty()
    {
        // Arrange
        var postDto = new AutoFaker<PostDto>()
            .Generate();

        var commentDtos = new AutoFaker<CommentDto>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid)
            .Generate(100).AsEnumerable();

        var getCommentsByPostGuidRequest = new AutoFaker<GetCommentsByPostGuidRequest>()
            .RuleFor(x => x.PostGuid, _ => postDto.Guid)
            .Generate();

        var getCommentsByPostGuidResponse = new GetCommentsByPostGuidMapper()
            .FromEntity(commentDtos);

        var result = Result.Ok(commentDtos);

        mockMediator.Setup(x => x.Send(It.IsAny<GetCommentsByPostGuidQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getCommentsByPostGuidEndpoint = Factory.Create<GetCommentsByPostGuidEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(GetCommentsByPostGuidRequest.PostGuid), getCommentsByPostGuidRequest.PostGuid), mockMediator.Object);

        // Act
        await getCommentsByPostGuidEndpoint.HandleAsync(getCommentsByPostGuidRequest, It.IsAny<CancellationToken>());

        // Assert
        getCommentsByPostGuidEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        getCommentsByPostGuidEndpoint.ValidationFailed.Should().BeFalse();
        getCommentsByPostGuidEndpoint.Response.Should().BeAssignableTo<IEnumerable<GetCommentsByPostGuidResponse>>().And.BeEquivalentTo(getCommentsByPostGuidResponse).And.NotBeEmpty();
    }
}
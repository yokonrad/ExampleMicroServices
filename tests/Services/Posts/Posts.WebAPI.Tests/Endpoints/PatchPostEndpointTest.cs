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

public class PatchPostEndpointTest
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
        var patchPostRequest = new AutoFaker<PatchPostRequest>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.Title, _ => It.IsAny<string>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>())
            .Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePartialPostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var patchPostEndpoint = Factory.Create<PatchPostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PatchPostRequest.Guid), patchPostRequest.Guid), mockMediator.Object);

        // Act
        await patchPostEndpoint.HandleAsync(patchPostRequest, It.IsAny<CancellationToken>());

        // Assert
        patchPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        patchPostEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_NotFoundError()
    {
        // Arrange
        var patchPostRequest = new AutoFaker<PatchPostRequest>()
            .Generate();

        var result = Result.Fail(new NotFoundError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePartialPostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var patchPostEndpoint = Factory.Create<PatchPostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PatchPostRequest.Guid), patchPostRequest.Guid), mockMediator.Object);

        // Act
        await patchPostEndpoint.HandleAsync(patchPostRequest, It.IsAny<CancellationToken>());

        // Assert
        patchPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        patchPostEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_SaveError()
    {
        // Arrange
        var patchPostRequest = new AutoFaker<PatchPostRequest>()
            .Generate();

        var result = Result.Fail(new SaveError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePartialPostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var patchPostEndpoint = Factory.Create<PatchPostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PatchPostRequest.Guid), patchPostRequest.Guid), mockMediator.Object);

        // Act
        await patchPostEndpoint.HandleAsync(patchPostRequest, It.IsAny<CancellationToken>());

        // Assert
        patchPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        patchPostEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var postDto = new AutoFaker<PostDto>()
            .Generate();

        var patchPostRequest = new AutoFaker<PatchPostRequest>()
            .RuleFor(x => x.Title, _ => postDto.Title)
            .RuleFor(x => x.Text, _ => postDto.Text)
            .RuleFor(x => x.Visible, _ => postDto.Visible)
            .Generate();

        var patchPostResponse = new PatchPostMapper()
            .FromEntity(postDto);

        var result = Result.Ok(postDto);

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePartialPostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var patchPostEndpoint = Factory.Create<PatchPostEndpoint>(mockMediator.Object);

        // Act
        await patchPostEndpoint.HandleAsync(patchPostRequest, It.IsAny<CancellationToken>());

        // Assert
        patchPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        patchPostEndpoint.ValidationFailed.Should().BeFalse();
        patchPostEndpoint.Response.Should().BeAssignableTo<PatchPostResponse>().And.BeEquivalentTo(patchPostResponse).And.NotBeNull();
    }
}
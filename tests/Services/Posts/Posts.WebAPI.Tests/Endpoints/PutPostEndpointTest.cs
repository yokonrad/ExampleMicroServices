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

public class PutPostEndpointTest
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
        var fakerPutPostRequest = new AutoFaker<PutPostRequest>()
            .RuleFor(x => x.Guid, _ => It.IsAny<Guid>())
            .RuleFor(x => x.Title, _ => It.IsAny<string>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>());

        var putPostRequest = fakerPutPostRequest.Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var putPostEndpoint = Factory.Create<PutPostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PutPostRequest.Guid), putPostRequest.Guid), mockMediator.Object);

        // Act
        await putPostEndpoint.HandleAsync(putPostRequest, It.IsAny<CancellationToken>());

        // Assert
        putPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        putPostEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_NotFoundError()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerPutPostRequest = new AutoFaker<PutPostRequest>()
            .RuleFor(x => x.Guid, _ => postDto.Guid);

        var putPostRequest = fakerPutPostRequest.Generate();

        var result = Result.Fail(new NotFoundError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var putPostEndpoint = Factory.Create<PutPostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PutPostRequest.Guid), putPostRequest.Guid), mockMediator.Object);

        // Act
        await putPostEndpoint.HandleAsync(putPostRequest, It.IsAny<CancellationToken>());

        // Assert
        putPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        putPostEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_SaveError()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerPutPostRequest = new AutoFaker<PutPostRequest>()
            .RuleFor(x => x.Title, _ => postDto.Title)
            .RuleFor(x => x.Text, _ => postDto.Text)
            .RuleFor(x => x.Visible, _ => postDto.Visible);

        var putPostRequest = fakerPutPostRequest.Generate();

        var result = Result.Fail(new SaveError());

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var putPostEndpoint = Factory.Create<PutPostEndpoint>(httpContext => httpContext.Request.RouteValues.Add(nameof(PutPostRequest.Guid), putPostRequest.Guid), mockMediator.Object);

        // Act
        await putPostEndpoint.HandleAsync(putPostRequest, It.IsAny<CancellationToken>());

        // Assert
        putPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        putPostEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerPutPostRequest = new AutoFaker<PutPostRequest>()
            .RuleFor(x => x.Title, _ => postDto.Title)
            .RuleFor(x => x.Text, _ => postDto.Text)
            .RuleFor(x => x.Visible, _ => postDto.Visible);

        var putPostRequest = fakerPutPostRequest.Generate();

        var putPostMapper = new PutPostMapper();
        var putPostResponse = putPostMapper.FromEntity(postDto);

        var result = Result.Ok(postDto);

        mockMediator.Setup(x => x.Send(It.IsAny<UpdatePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var putPostEndpoint = Factory.Create<PutPostEndpoint>(mockMediator.Object);

        // Act
        await putPostEndpoint.HandleAsync(putPostRequest, It.IsAny<CancellationToken>());

        var putPostEndpointResponse = putPostEndpoint.Response;

        // Assert
        putPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        putPostEndpoint.ValidationFailed.Should().BeFalse();
        putPostEndpointResponse.Should().BeAssignableTo<PutPostResponse>().And.BeEquivalentTo(putPostResponse).And.NotBeNull();
    }
}
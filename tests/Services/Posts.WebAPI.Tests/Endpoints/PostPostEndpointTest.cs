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

public class PostPostEndpointTest
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
        var fakerPostPostRequest = new AutoFaker<PostPostRequest>()
            .RuleFor(x => x.Title, _ => It.IsAny<string>())
            .RuleFor(x => x.Text, _ => It.IsAny<string>())
            .RuleFor(x => x.Visible, _ => It.IsAny<bool>());

        var postPostRequest = fakerPostPostRequest.Generate();

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var postPostEndpoint = Factory.Create<PostPostEndpoint>(mockMediator.Object);

        // Act
        await postPostEndpoint.HandleAsync(postPostRequest, It.IsAny<CancellationToken>());

        // Assert
        postPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        postPostEndpoint.ValidationFailed.Should().BeTrue();
    }

    [Test]
    public async Task Should_Be_Invalid_When_Result_SaveError()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerPostPostRequest = new AutoFaker<PostPostRequest>()
            .RuleFor(x => x.Title, _ => postDto.Title)
            .RuleFor(x => x.Text, _ => postDto.Text)
            .RuleFor(x => x.Visible, _ => postDto.Visible);

        var postPostRequest = fakerPostPostRequest.Generate();

        var result = Result.Fail(new SaveError());

        mockMediator.Setup(x => x.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var postPostEndpoint = Factory.Create<PostPostEndpoint>(mockMediator.Object);

        // Act
        await postPostEndpoint.HandleAsync(postPostRequest, It.IsAny<CancellationToken>());

        // Assert
        postPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        postPostEndpoint.ValidationFailed.Should().BeFalse();
    }

    [Test]
    public async Task Should_Be_Valid()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDto = fakerPostDto.Generate();

        var fakerPostPostRequest = new AutoFaker<PostPostRequest>()
            .RuleFor(x => x.Title, _ => postDto.Title)
            .RuleFor(x => x.Text, _ => postDto.Text)
            .RuleFor(x => x.Visible, _ => postDto.Visible);

        var postPostRequest = fakerPostPostRequest.Generate();

        var postPostMapper = new PostPostMapper();
        var postPostResponse = postPostMapper.FromEntity(postDto);

        var result = Result.Ok(postDto);

        mockMediator.Setup(x => x.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var postPostEndpoint = Factory.Create<PostPostEndpoint>(mockMediator.Object);

        // Act
        await postPostEndpoint.HandleAsync(postPostRequest, It.IsAny<CancellationToken>());

        var postPostEndpointResponse = postPostEndpoint.Response;

        // Assert
        postPostEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        postPostEndpoint.ValidationFailed.Should().BeFalse();
        postPostEndpointResponse.Should().BeAssignableTo<PostPostResponse>().And.BeEquivalentTo(postPostResponse).And.NotBeNull();
    }
}
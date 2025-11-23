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

public class GetPostsEndpointTest
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
        var postDtos = Array.Empty<PostDto>().AsEnumerable();

        var getPostsMapper = new GetPostsMapper();
        var getPostsResponse = getPostsMapper.FromEntity(postDtos);

        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        mockMediator.Setup(x => x.Send(It.IsAny<GetPostsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getPostsEndpoint = Factory.Create<GetPostsEndpoint>(mockMediator.Object);

        // Act
        await getPostsEndpoint.HandleAsync(It.IsAny<CancellationToken>());

        var getPostsEndpointResponse = getPostsEndpoint.Response;

        // Assert
        getPostsEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        getPostsEndpoint.ValidationFailed.Should().BeTrue();
        getPostsEndpointResponse.Should().BeAssignableTo<IEnumerable<GetPostsResponse>>().And.BeEquivalentTo(getPostsResponse).And.BeEmpty();
    }

    [Test]
    public async Task Should_Be_Valid_When_Result_Empty()
    {
        // Arrange
        var postDtos = Array.Empty<PostDto>().AsEnumerable();

        var getPostsMapper = new GetPostsMapper();
        var getPostsResponse = getPostsMapper.FromEntity(postDtos);

        var result = Result.Ok(postDtos);

        mockMediator.Setup(x => x.Send(It.IsAny<GetPostsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getPostsEndpoint = Factory.Create<GetPostsEndpoint>(mockMediator.Object);

        // Act
        await getPostsEndpoint.HandleAsync(It.IsAny<CancellationToken>());

        var getPostsEndpointResponse = getPostsEndpoint.Response;

        // Assert
        getPostsEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        getPostsEndpoint.ValidationFailed.Should().BeFalse();
        getPostsEndpointResponse.Should().BeAssignableTo<IEnumerable<GetPostsResponse>>().And.BeEquivalentTo(getPostsResponse).And.BeEmpty();
    }

    [Test]
    public async Task Should_Be_Valid_When_Result_Not_Empty()
    {
        // Arrange
        var fakerPostDto = new AutoFaker<PostDto>();
        var postDtos = fakerPostDto.Generate(100).AsEnumerable();

        var getPostsMapper = new GetPostsMapper();
        var getPostsResponse = getPostsMapper.FromEntity(postDtos);

        var result = Result.Ok(postDtos);

        mockMediator.Setup(x => x.Send(It.IsAny<GetPostsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        var getPostsEndpoint = Factory.Create<GetPostsEndpoint>(mockMediator.Object);

        // Act
        await getPostsEndpoint.HandleAsync(It.IsAny<CancellationToken>());

        var getPostsEndpointResponse = getPostsEndpoint.Response;

        // Assert
        getPostsEndpoint.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        getPostsEndpoint.ValidationFailed.Should().BeFalse();
        getPostsEndpointResponse.Should().BeAssignableTo<IEnumerable<GetPostsResponse>>().And.BeEquivalentTo(getPostsResponse).And.NotBeEmpty();
    }
}
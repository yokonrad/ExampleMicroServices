using AutoBogus;
using Core.Application.Errors;
using Core.WebAPI.Extensions;
using Core.WebAPI.Tests.Examples;
using FastEndpoints;
using FluentAssertions;
using FluentResults;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Core.WebAPI.Tests.Extensions;

public class FastEndpointsExtensionTest
{
    private Mock<IMediator> mockMediator;

    [SetUp]
    public async Task SetUp()
    {
        mockMediator = new Mock<IMediator>();
    }

    [Test]
    public async Task SendResponse_Should_Be_Status404NotFound_When_NotFoundError()
    {
        // Arrange
        var result = Result.Fail(new NotFoundError());

        var exampleEndpointMapper = new ExampleEndpointMapper();
        var exampleEndpoint = Factory.Create<ExampleEndpoint>(mockMediator.Object);
        var exampleEndpointResponseSender = new ResponseSender<ExampleEndpointRequest, ExampleEndpointResponse>(exampleEndpoint);

        // Act
        await exampleEndpointResponseSender.SendResponse<ExampleEndpointRequest, ExampleEndpointResponse, string>(result, exampleEndpointMapper.FromEntity);

        // Assert
        exampleEndpointResponseSender.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task SendResponse_Should_Be_Status500InternalServerError_When_SaveError()
    {
        // Arrange
        var result = Result.Fail(new SaveError());

        var exampleEndpointMapper = new ExampleEndpointMapper();
        var exampleEndpoint = Factory.Create<ExampleEndpoint>(mockMediator.Object);
        var exampleEndpointResponseSender = new ResponseSender<ExampleEndpointRequest, ExampleEndpointResponse>(exampleEndpoint);

        // Act
        await exampleEndpointResponseSender.SendResponse<ExampleEndpointRequest, ExampleEndpointResponse, string>(result, exampleEndpointMapper.FromEntity);

        // Assert
        exampleEndpointResponseSender.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Test]
    public async Task SendResponse_Should_Be_Status400BadRequest_When_ValidationError()
    {
        // Arrange
        var result = Result.Fail([new ValidationError("Property name", "Error message")]);

        var exampleEndpointMapper = new ExampleEndpointMapper();
        var exampleEndpoint = Factory.Create<ExampleEndpoint>(mockMediator.Object);
        var exampleEndpointResponseSender = new ResponseSender<ExampleEndpointRequest, ExampleEndpointResponse>(exampleEndpoint);

        // Act
        await exampleEndpointResponseSender.SendResponse<ExampleEndpointRequest, ExampleEndpointResponse, string>(result, exampleEndpointMapper.FromEntity);

        // Assert
        exampleEndpointResponseSender.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task SendResponse_Should_Be_Status500InternalServerError_When_Failed()
    {
        // Arrange
        var result = Result.Fail(It.IsAny<string>());

        var exampleEndpointMapper = new ExampleEndpointMapper();
        var exampleEndpoint = Factory.Create<ExampleEndpoint>(mockMediator.Object);
        var exampleEndpointResponseSender = new ResponseSender<ExampleEndpointRequest, ExampleEndpointResponse>(exampleEndpoint);
        exampleEndpointResponseSender.ValidationFailures.Add(new ValidationFailure("Property name", "Error message"));

        // Act
        await exampleEndpointResponseSender.SendResponse<ExampleEndpointRequest, ExampleEndpointResponse, string>(result, exampleEndpointMapper.FromEntity);

        // Assert
        exampleEndpointResponseSender.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Test]
    public async Task SendResponse_Should_Be_Status204NoContent()
    {
        // Arrange
        var exampleEndpointMapper = new ExampleEndpointMapper();
        var exampleEndpoint = Factory.Create<ExampleEndpoint>(mockMediator.Object);
        var exampleEndpointResponseSender = new ResponseSender<ExampleEndpointRequest, ExampleEndpointResponse>(exampleEndpoint);

        // Act
        await exampleEndpointResponseSender.SendResponse<ExampleEndpointRequest, ExampleEndpointResponse, string>(It.IsAny<string>(), exampleEndpointMapper.FromEntity);

        // Assert
        exampleEndpointResponseSender.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Test]
    public async Task SendResponse_Should_Be_Status200OK()
    {
        // Arrange
        var fakerExampleRequest = new AutoFaker<ExampleRequest>();
        var exampleRequest = fakerExampleRequest.Generate();

        var exampleRequestHandler = new ExampleRequestHandler();
        var exampleRequestHandlerResult = await exampleRequestHandler.Handle(exampleRequest, It.IsAny<CancellationToken>());

        var exampleEndpointMapper = new ExampleEndpointMapper();
        var exampleEndpoint = Factory.Create<ExampleEndpoint>(mockMediator.Object);
        var exampleEndpointResponseSender = new ResponseSender<ExampleEndpointRequest, ExampleEndpointResponse>(exampleEndpoint);

        // Act
        await exampleEndpointResponseSender.SendResponse(exampleRequestHandlerResult, exampleEndpointMapper.FromEntity);

        // Assert
        exampleEndpointResponseSender.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }
}
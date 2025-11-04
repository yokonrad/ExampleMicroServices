using AutoBogus;
using Core.Application.Behaviors;
using Core.Application.Tests.Examples;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Core.Application.Tests.Behaviors;

public class PerformanceBehaviorTest
{
    private Mock<ILogger<PerformanceBehavior<ExampleRequest, bool>>> mockLogger;
    private PerformanceBehavior<ExampleRequest, bool> performanceBehavior;
    private ExampleRequestHandler exampleRequestHandler;

    [SetUp]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<PerformanceBehavior<ExampleRequest, bool>>>();
        performanceBehavior = new PerformanceBehavior<ExampleRequest, bool>(mockLogger.Object);
        exampleRequestHandler = new ExampleRequestHandler();
    }

    [Test]
    public async Task Should_Be_Valid_Verify_LogInformation_Exactly_Two()
    {
        // Arrange
        var fakerExampleRequest = new AutoFaker<ExampleRequest>();
        var exampleRequest = fakerExampleRequest.Generate();

        // Act
        var act = await performanceBehavior.Handle(exampleRequest, (x) => exampleRequestHandler.Handle(exampleRequest, x), It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeTrue();
        mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));
    }
}
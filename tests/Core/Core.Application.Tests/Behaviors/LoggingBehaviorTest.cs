using AutoBogus;
using Core.Application.Behaviors;
using Core.Application.Tests.Examples;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Core.Application.Tests.Behaviors;

public class LoggingBehaviorTest
{
    private Mock<ILogger<LoggingBehavior<ExampleRequest, bool>>> mockLogger;
    private LoggingBehavior<ExampleRequest, bool> loggingBehavior;
    private ExampleRequestHandler exampleRequestHandler;

    [SetUp]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<LoggingBehavior<ExampleRequest, bool>>>();
        loggingBehavior = new LoggingBehavior<ExampleRequest, bool>(mockLogger.Object);
        exampleRequestHandler = new ExampleRequestHandler();
    }

    [Test]
    public async Task Should_Be_Valid_Verify_LogInformation_Exactly_Two()
    {
        // Arrange
        var exampleRequest = new AutoFaker<ExampleRequest>()
            .Generate();

        // Act
        var act = await loggingBehavior.Handle(exampleRequest, (x) => exampleRequestHandler.Handle(exampleRequest, x), It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeTrue();
        mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));
    }
}
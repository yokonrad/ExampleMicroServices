using AutoBogus;
using Core.Application.Behaviors;
using Core.Application.Tests.Examples;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Core.Application.Tests.Behaviors;

public class ExceptionBehaviorTest
{
    private Mock<ILogger<ExceptionBehavior<ExampleRequest, bool>>> mockLogger;
    private ExceptionBehavior<ExampleRequest, bool> exceptionBehavior;
    private ExampleRequestHandler exampleRequestHandler;

    [SetUp]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<ExceptionBehavior<ExampleRequest, bool>>>();
        exceptionBehavior = new ExceptionBehavior<ExampleRequest, bool>(mockLogger.Object);
        exampleRequestHandler = new ExampleRequestHandler();
    }

    [Test]
    public async Task Should_Be_Invalid_Verify_LogError_Once()
    {
        // Arrange
        var exampleRequest = new AutoFaker<ExampleRequest>()
            .RuleFor(x => x.Example, _ => It.IsAny<string>())
            .Generate();

        // Act
        var act = await exceptionBehavior.Handle(exampleRequest, (x) => exampleRequestHandler.Handle(exampleRequest, x), It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeFalse();
        mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once());
    }

    [Test]
    public async Task Should_Be_Valid_Verify_LogInformation_Exactly_Two_LogError_Never()
    {
        // Arrange
        var exampleRequest = new AutoFaker<ExampleRequest>()
            .Generate();

        // Act
        var act = await exceptionBehavior.Handle(exampleRequest, (x) => exampleRequestHandler.Handle(exampleRequest, x), It.IsAny<CancellationToken>());

        // Assert
        act.Should().BeTrue();
        mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));
        mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never());
    }
}
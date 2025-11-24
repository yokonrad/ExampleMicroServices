using Microsoft.Extensions.Logging;
using Moq;
using Posts.Application.Dtos;
using Posts.Application.Notifications;

namespace Posts.Application.Tests.Notifications;

public class CreatedPostNotificationTest
{
    [Test]
    public async Task Verify_LogInformation_Once()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<CreatedPostNotificationHandler>>();
        var createdPostNotificationHandler = new CreatedPostNotificationHandler(mockLogger.Object);
        var createdPostNotification = new CreatedPostNotification(It.IsAny<PostDto>());

        // Act
        await createdPostNotificationHandler.Handle(createdPostNotification, It.IsAny<CancellationToken>());

        // Assert
        mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once());
    }
}
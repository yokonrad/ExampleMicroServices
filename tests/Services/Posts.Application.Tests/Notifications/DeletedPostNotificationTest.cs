using Microsoft.Extensions.Logging;
using Moq;
using Posts.Application.Dtos;
using Posts.Application.Notifications;

namespace Posts.Application.Tests.Notifications;

public class DeletedPostNotificationTest
{
    [Test]
    public async Task Verify_LogInformation_Once()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DeletedPostNotificationHandler>>();
        var deletedPostNotificationHandler = new DeletedPostNotificationHandler(mockLogger.Object);
        var deletedPostNotification = new DeletedPostNotification(It.IsAny<PostDto>());

        // Act
        await deletedPostNotificationHandler.Handle(deletedPostNotification, It.IsAny<CancellationToken>());

        // Assert
        mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once());
    }
}
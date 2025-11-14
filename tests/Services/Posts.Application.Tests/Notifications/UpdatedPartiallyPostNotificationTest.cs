using Microsoft.Extensions.Logging;
using Moq;
using Posts.Application.Dtos;
using Posts.Application.Notifications;

namespace Posts.Application.Tests.Notifications;

public class UpdatedPartiallyPostNotificationTest
{
    [Test]
    public async Task Verify_LogInformation_Once()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UpdatedPartiallyPostNotificationHandler>>();
        var updatedPartiallyPostNotificationHandler = new UpdatedPartiallyPostNotificationHandler(mockLogger.Object);
        var updatedPartiallyPostNotification = new UpdatedPartiallyPostNotification(It.IsAny<PostDto>());

        // Act
        await updatedPartiallyPostNotificationHandler.Handle(updatedPartiallyPostNotification, It.IsAny<CancellationToken>());

        // Assert
        mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once());
    }
}
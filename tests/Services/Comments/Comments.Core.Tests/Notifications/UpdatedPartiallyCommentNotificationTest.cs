using Comments.Core.Dtos;
using Comments.Core.Notifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Comments.Core.Tests.Notifications;

public class UpdatedPartiallyCommentNotificationTest
{
    [Test]
    public async Task Verify_LogInformation_Once()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UpdatedCommentNotificationHandler>>();
        var updatedCommentNotificationHandler = new UpdatedCommentNotificationHandler(mockLogger.Object);
        var updatedCommentNotification = new UpdatedCommentNotification(It.IsAny<CommentDto>());

        // Act
        await updatedCommentNotificationHandler.Handle(updatedCommentNotification, It.IsAny<CancellationToken>());

        // Assert
        mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once());
    }
}
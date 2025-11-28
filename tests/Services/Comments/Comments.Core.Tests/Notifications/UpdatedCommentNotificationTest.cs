using Comments.Core.Dtos;
using Comments.Core.Notifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Comments.Core.Tests.Notifications;

public class UpdatedCommentNotificationTest
{
    [Test]
    public async Task Verify_LogInformation_Once()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UpdatedPartiallyCommentNotificationHandler>>();
        var updatedPartiallyCommentNotificationHandler = new UpdatedPartiallyCommentNotificationHandler(mockLogger.Object);
        var updatedPartiallyCommentNotification = new UpdatedPartiallyCommentNotification(It.IsAny<CommentDto>());

        // Act
        await updatedPartiallyCommentNotificationHandler.Handle(updatedPartiallyCommentNotification, It.IsAny<CancellationToken>());

        // Assert
        mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once());
    }
}
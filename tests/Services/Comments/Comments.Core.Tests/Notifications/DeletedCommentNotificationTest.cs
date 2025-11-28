using Comments.Core.Dtos;
using Comments.Core.Notifications;
using Microsoft.Extensions.Logging;
using Moq;

namespace Comments.Core.Tests.Notifications;

public class DeletedCommentNotificationTest
{
    [Test]
    public async Task Verify_LogInformation_Once()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DeletedCommentNotificationHandler>>();
        var deletedCommentNotificationHandler = new DeletedCommentNotificationHandler(mockLogger.Object);
        var deletedCommentNotification = new DeletedCommentNotification(It.IsAny<CommentDto>());

        // Act
        await deletedCommentNotificationHandler.Handle(deletedCommentNotification, It.IsAny<CancellationToken>());

        // Assert
        mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once());
    }
}
using Comments.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comments.Core.Notifications;

public record DeletedCommentNotification(CommentDto CommentDto) : INotification;

public class DeletedCommentNotificationHandler(ILogger<DeletedCommentNotificationHandler> logger) : INotificationHandler<DeletedCommentNotification>
{
    public async Task Handle(DeletedCommentNotification createdPostNotification, CancellationToken cancellationToken)
    {
        await Task.Run(() => logger.LogInformation("Handling notification with name: {RequestName}", typeof(DeletedCommentNotification).Name), cancellationToken);
    }
}
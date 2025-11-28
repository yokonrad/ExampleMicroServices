using Comments.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comments.Core.Notifications;

public record UpdatedCommentNotification(CommentDto CommentDto) : INotification;

public class UpdatedCommentNotificationHandler(ILogger<UpdatedCommentNotificationHandler> logger) : INotificationHandler<UpdatedCommentNotification>
{
    public async Task Handle(UpdatedCommentNotification createdPostNotification, CancellationToken cancellationToken)
    {
        await Task.Run(() => logger.LogInformation("Handling notification with name: {RequestName}", typeof(UpdatedCommentNotification).Name), cancellationToken);
    }
}
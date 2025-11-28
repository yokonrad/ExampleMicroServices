using Comments.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comments.Core.Notifications;

public record CreatedCommentNotification(CommentDto CommentDto) : INotification;

public class CreatedCommentNotificationHandler(ILogger<CreatedCommentNotificationHandler> logger) : INotificationHandler<CreatedCommentNotification>
{
    public async Task Handle(CreatedCommentNotification createdPostNotification, CancellationToken cancellationToken)
    {
        await Task.Run(() => logger.LogInformation("Handling notification with name: {RequestName}", typeof(CreatedCommentNotification).Name), cancellationToken);
    }
}
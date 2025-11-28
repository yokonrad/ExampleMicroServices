using Comments.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Comments.Core.Notifications;

public record UpdatedPartiallyCommentNotification(CommentDto CommentDto) : INotification;

public class UpdatedPartiallyCommentNotificationHandler(ILogger<UpdatedPartiallyCommentNotificationHandler> logger) : INotificationHandler<UpdatedPartiallyCommentNotification>
{
    public async Task Handle(UpdatedPartiallyCommentNotification createdPostNotification, CancellationToken cancellationToken)
    {
        await Task.Run(() => logger.LogInformation("Handling notification with name: {RequestName}", typeof(UpdatedPartiallyCommentNotification).Name), cancellationToken);
    }
}
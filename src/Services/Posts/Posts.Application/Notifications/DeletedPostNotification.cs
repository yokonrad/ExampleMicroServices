using MediatR;
using Microsoft.Extensions.Logging;
using Posts.Application.Dtos;

namespace Posts.Application.Notifications;

public record DeletedPostNotification(PostDto PostDto) : INotification;

public class DeletedPostNotificationHandler(ILogger<DeletedPostNotificationHandler> logger) : INotificationHandler<DeletedPostNotification>
{
    public async Task Handle(DeletedPostNotification deletedPostNotification, CancellationToken cancellationToken)
    {
        await Task.Run(() => logger.LogInformation("Handling notification with name: {RequestName}", typeof(DeletedPostNotification).Name), cancellationToken);
    }
}
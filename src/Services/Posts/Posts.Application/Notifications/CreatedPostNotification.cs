using MediatR;
using Microsoft.Extensions.Logging;
using Posts.Application.Dtos;

namespace Posts.Application.Notifications;

public record CreatedPostNotification(PostDto PostDto) : INotification;

public class CreatedPostNotificationHandler(ILogger<CreatedPostNotificationHandler> logger) : INotificationHandler<CreatedPostNotification>
{
    public async Task Handle(CreatedPostNotification createdPostNotification, CancellationToken cancellationToken)
    {
        await Task.Run(() => logger.LogInformation("Handling notification with name: {RequestName}", typeof(CreatedPostNotification).Name), cancellationToken);
    }
}
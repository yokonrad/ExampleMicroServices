using MediatR;
using Microsoft.Extensions.Logging;
using Posts.Application.Dtos;

namespace Posts.Application.Notifications;

public record UpdatedPostNotification(PostDto PostDto) : INotification;

public class UpdatedPostNotificationHandler(ILogger<UpdatedPostNotificationHandler> logger) : INotificationHandler<UpdatedPostNotification>
{
    public async Task Handle(UpdatedPostNotification updatedPostNotification, CancellationToken cancellationToken)
    {
        await Task.Run(() => logger.LogInformation("Handling notification with name: {RequestName}", typeof(UpdatedPostNotification).Name), cancellationToken);
    }
}
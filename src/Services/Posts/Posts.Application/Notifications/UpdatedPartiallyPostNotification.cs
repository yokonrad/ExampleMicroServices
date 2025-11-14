using MediatR;
using Microsoft.Extensions.Logging;
using Posts.Application.Dtos;

namespace Posts.Application.Notifications;

public record UpdatedPartiallyPostNotification(PostDto PostDto) : INotification;

public class UpdatedPartiallyPostNotificationHandler(ILogger<UpdatedPartiallyPostNotificationHandler> logger) : INotificationHandler<UpdatedPartiallyPostNotification>
{
    public async Task Handle(UpdatedPartiallyPostNotification updatedPartiallyPostNotification, CancellationToken cancellationToken)
    {
        await Task.Run(() => logger.LogInformation("Handling notification with name: {RequestName}", typeof(UpdatedPartiallyPostNotification).Name), cancellationToken);
    }
}
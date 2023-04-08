using BaseBotService.Core.Messages;
using Discord.WebSocket;

namespace BaseBotService.Interactions;
public class ActivityServiceHandler : INotificationHandler<UpdateActivityNotification>
{
    private readonly DiscordSocketClient _client;

    public ActivityServiceHandler(DiscordSocketClient client)
    {
        _client = client;
    }

    public async Task Handle(UpdateActivityNotification notification, CancellationToken cancellationToken)
    {
        await _client.SetActivityAsync(new Game(notification.Description, notification.ActivityType));
        await _client.SetStatusAsync(notification.Status);
    }
}
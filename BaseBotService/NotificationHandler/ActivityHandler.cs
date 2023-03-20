using BaseBotService.Notifications;
using Discord.WebSocket;

namespace BaseBotService.NotificationHandler;
public class ActivityHandler : INotificationHandler<ClientReadyNotification>
{
    private readonly DiscordSocketClient _client;

    public ActivityHandler(DiscordSocketClient client)
    {
        _client = client;
    }

    public async Task Handle(ClientReadyNotification notification, CancellationToken cancellationToken)
    {
        await _client.SetActivityAsync(new Game("the world burn", ActivityType.Watching));
        await _client.SetStatusAsync(UserStatus.Online);
    }
}

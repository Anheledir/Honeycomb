using BaseBotService.Messages;
using Discord.WebSocket;

namespace BaseBotService.Notifications;
public class ActivityHandler : INotificationHandler<ClientReadyNotification>
{
    private readonly DiscordSocketClient _client;

    public ActivityHandler(DiscordSocketClient client)
    {
        _client = client;
    }

    // TODO: Move this into its own service.
    public async Task Handle(ClientReadyNotification notification, CancellationToken cancellationToken)
    {
        await _client.SetActivityAsync(new Game("the world burn", ActivityType.Watching));
        await _client.SetStatusAsync(UserStatus.Online);
    }
}

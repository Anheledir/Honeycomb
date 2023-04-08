using BaseBotService.Core.Messages;
using Discord.WebSocket;

namespace BaseBotService.Interactions;
public class ActivityServiceHandler : INotificationHandler<UpdateActivityNotification>
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;

    public ActivityServiceHandler(ILogger logger, DiscordSocketClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Handle(UpdateActivityNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug($"{nameof(ActivityServiceHandler)} received {nameof(UpdateActivityNotification)}");
        await _client.SetActivityAsync(new Game(notification.Description, notification.ActivityType));
        await _client.SetStatusAsync(notification.Status);
    }
}
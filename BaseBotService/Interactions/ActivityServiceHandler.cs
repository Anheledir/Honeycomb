using BaseBotService.Core;
using Discord.WebSocket;

namespace BaseBotService.Interactions;

/// <summary>
/// Handles activity update notifications for the Discord bot.
/// </summary>
public class ActivityServiceHandler : INotificationHandler<DiscordEventListener.UpdateActivityNotification>
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityServiceHandler"/> class.
    /// </summary>
    public ActivityServiceHandler(ILogger logger, DiscordSocketClient client)
    {
        _logger = logger.ForContext<ActivityServiceHandler>();
        _client = client;
    }

    /// <summary>
    /// Handles the <see cref="UpdateActivityNotification"/> to update the bot's activity and status.
    /// </summary>
    /// <param name="notification">The notification containing activity and status details.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    public async Task Handle(DiscordEventListener.UpdateActivityNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug("{Handler} received {Notification} with Activity: {Activity}, Status: {Status}",
            nameof(ActivityServiceHandler), nameof(DiscordEventListener.UpdateActivityNotification), notification.Description, notification.Status);

        try
        {
            await _client.SetStatusAsync(notification.Status);
            await _client.SetCustomStatusAsync(notification.Description);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update bot activity or status.");
        }
    }
}

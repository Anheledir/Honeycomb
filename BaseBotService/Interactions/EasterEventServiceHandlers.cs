using BaseBotService.Core.Interfaces;
using BaseBotService.Core.Messages;

namespace BaseBotService.Interactions;

/// <summary>
/// Handles Easter event notifications related to messages received and reactions added.
/// </summary>
public class EasterEventServiceHandlers : INotificationHandler<MessageReceivedNotification>, INotificationHandler<ReactionAddedNotification>
{
    private readonly ILogger _logger;
    private readonly IEasterEventService _easterEvent;

    /// <summary>
    /// Initializes a new instance of the <see cref="EasterEventServiceHandlers"/> class.
    /// </summary>
    public EasterEventServiceHandlers(ILogger logger, IEasterEventService easterEvent)
    {
        _logger = logger.ForContext<EasterEventServiceHandlers>();
        _easterEvent = easterEvent;
    }

    /// <summary>
    /// Handles the MessageReceivedNotification event and triggers the Easter event service.
    /// </summary>
    public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug("{Handler} received {Notification}", nameof(EasterEventServiceHandlers), nameof(MessageReceivedNotification));

        try
        {
            await _easterEvent.HandleMessageReceivedAsync(notification);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error handling {Notification} in {Handler}", nameof(MessageReceivedNotification), nameof(EasterEventServiceHandlers));
        }
    }

    /// <summary>
    /// Handles the ReactionAddedNotification event and triggers the Easter event service.
    /// </summary>
    public async Task Handle(ReactionAddedNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug("{Handler} received {Notification}", nameof(EasterEventServiceHandlers), nameof(ReactionAddedNotification));

        try
        {
            await _easterEvent.HandleReactionAddedAsync(notification);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error handling {Notification} in {Handler}", nameof(ReactionAddedNotification), nameof(EasterEventServiceHandlers));
        }
    }
}

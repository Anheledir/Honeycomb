using BaseBotService.Core.Interfaces;
using BaseBotService.Core.Messages;

namespace BaseBotService.Interactions;
public class EasterEventServiceHandlers : INotificationHandler<MessageReceivedNotification>, INotificationHandler<ReactionAddedNotification>
{
    private readonly ILogger _logger;
    private readonly IEasterEventService _easterEvent;

    public EasterEventServiceHandlers(ILogger logger, IEasterEventService easterEvent)
    {
        _logger = logger.ForContext<EasterEventServiceHandlers>();
        _easterEvent = easterEvent;
    }

    public Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug($"{nameof(EasterEventServiceHandlers)} received {nameof(MessageReceivedNotification)}");
        _easterEvent.HandleMessageReceivedAsync(notification);
        return Task.CompletedTask;
    }

    public Task Handle(ReactionAddedNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug($"{nameof(EasterEventServiceHandlers)} received {nameof(ReactionAddedNotification)}");
        _easterEvent.HandleReactionAddedAsync(notification);
        return Task.CompletedTask;
    }
}

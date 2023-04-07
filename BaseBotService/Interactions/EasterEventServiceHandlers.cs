using BaseBotService.Core.Messages;
using BaseBotService.Infrastructure.Services;

namespace BaseBotService.Interactions;
public class EasterEventServiceHandlers : INotificationHandler<MessageReceivedNotification>, INotificationHandler<ReactionAddedNotification>
{
    private readonly ILogger _logger;
    private readonly EasterEventService _easterEvent;

    public EasterEventServiceHandlers(ILogger logger, EasterEventService easterEvent)
    {
        _logger = logger.ForContext<EasterEventServiceHandlers>();
        _easterEvent = easterEvent;
    }

    public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug("{handler} received {@notification}", nameof(EasterEventServiceHandlers), notification);
        await _easterEvent.HandleMessageReceivedAsync(notification);
    }

    public async Task Handle(ReactionAddedNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug("{handler} received {@notification}", nameof(EasterEventServiceHandlers), notification);
        await _easterEvent.HandleReactionAddedAsync(notification);
    }
}

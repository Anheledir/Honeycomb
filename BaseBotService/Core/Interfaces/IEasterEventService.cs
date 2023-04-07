using BaseBotService.Core.Messages;

namespace BaseBotService.Core.Interfaces;
public interface IEasterEventService
{
    Task HandleMessageReceivedAsync(MessageReceivedNotification arg);
    Task HandleReactionAddedAsync(ReactionAddedNotification arg);
}
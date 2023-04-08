using Discord.WebSocket;

namespace BaseBotService.Core.Messages;
public class ReactionAddedNotification : INotification
{
    public ReactionAddedNotification(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        Message = message;
        Channel = channel;
        Reaction = reaction;
    }

    public Cacheable<IUserMessage, ulong> Message { get; }
    public Cacheable<IMessageChannel, ulong> Channel { get; }
    public SocketReaction Reaction { get; }
}
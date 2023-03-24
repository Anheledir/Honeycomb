namespace BaseBotService.Core.Messages;

public class MessageComponentRequest : IRequest
{
    public MessageComponentRequest(SocketInteractionContext ctx)
    {
        Context = ctx;
    }

    public SocketInteractionContext Context { get; }
}
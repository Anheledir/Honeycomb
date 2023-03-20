namespace BaseBotService.Messages;

internal class MessageComponentRequest : IRequest
{
    public MessageComponentRequest(SocketInteractionContext ctx)
    {
        Context = ctx;
    }

    public SocketInteractionContext Context { get; }
}
namespace BaseBotService.Messages;

internal class ApplicationCommandRequest : IRequest
{
    public ApplicationCommandRequest(SocketInteractionContext ctx)
    {
        Context = ctx;
    }
    public SocketInteractionContext Context { get; }
}
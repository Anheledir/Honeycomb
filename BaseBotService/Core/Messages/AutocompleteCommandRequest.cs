namespace BaseBotService.Core.Messages;

internal class AutocompleteCommandRequest : IRequest
{
    public AutocompleteCommandRequest(SocketInteractionContext context)
    {
        Context = context;
    }

    public SocketInteractionContext Context { get; }
}
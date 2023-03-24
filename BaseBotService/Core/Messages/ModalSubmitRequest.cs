namespace BaseBotService.Core.Messages;

public class ModalSubmitRequest : IRequest
{
    public ModalSubmitRequest(SocketInteractionContext context)
    {
        Context = context;
    }

    public SocketInteractionContext Context { get; }
}
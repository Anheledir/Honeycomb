using Discord.WebSocket;

namespace BaseBotService.Messages;
public class InteractionCreatedNotification : INotification
{
    public SocketInteraction Interaction { get; set; }

    public InteractionCreatedNotification(SocketInteraction interaction)
    {
        Interaction = interaction;
    }
}
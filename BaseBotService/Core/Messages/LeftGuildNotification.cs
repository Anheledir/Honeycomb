using Discord.WebSocket;

namespace BaseBotService.Core.Messages;
public class LeftGuildNotification : INotification
{
    public SocketGuild Guild { get; set; }

    public LeftGuildNotification(SocketGuild guild)
    {
        Guild = guild;
    }
}
using Discord.WebSocket;

namespace BaseBotService.Core.Messages;
public class JoinedGuildNotification : INotification
{
    public JoinedGuildNotification(SocketGuild guild)
    {
        Guild = guild;
    }

    public SocketGuild Guild { get; set; }
}
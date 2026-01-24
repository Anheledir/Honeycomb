using Discord.WebSocket;

namespace BaseBotService.Core.Messages;
public class UserLeftNotification : INotification
{
    public SocketGuild Guild { get; }
    public SocketUser User { get; }

    public UserLeftNotification(SocketGuild guild, SocketUser user)
    {
        Guild = guild;
        User = user;
    }
}

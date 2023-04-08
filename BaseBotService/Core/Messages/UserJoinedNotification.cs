using Discord.WebSocket;

namespace BaseBotService.Core.Messages;
public class UserJoinedNotification : INotification
{
    public SocketGuildUser User { get; set; }

    public UserJoinedNotification(SocketGuildUser user)
    {
        User = user;
    }
}
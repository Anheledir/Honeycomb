using Discord.WebSocket;

namespace BaseBotService.Core.Interfaces;
public interface IPermissionService
{
    bool CanUserExecuteCommandAsync(SocketGuildUser user, SocketGuild guild);
    List<ulong> GetModeratorRoles(SocketGuild guild);
    bool IsUserAdmin(SocketGuildUser user);
    bool IsUserModerator(SocketGuildUser user, SocketRole modRoles);
    void SetModeratorRole(SocketGuild guild, SocketRole role);
}
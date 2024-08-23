using Discord.WebSocket;

namespace BaseBotService.Core.Interfaces;
public interface IPermissionService
{
    bool CanUserExecuteModeratorCommand(SocketGuildUser? user);
    bool CanUserExecuteArtistCommand(SocketGuildUser? user);
    List<ulong> GetModeratorRoles(SocketGuild guild);
    bool IsUserAdmin(SocketGuildUser user);
    bool IsUserModerator(SocketGuildUser user, SocketRole modRoles);
    void SetModeratorRole(SocketGuild guild, SocketRole role);
    Task<bool> CanUserExecuteModeratorCommandAsync(SocketGuildUser? user);
    Task<bool> CanUserExecuteArtistCommandAsync(SocketGuildUser? user);
    SocketRole GetModeratorRole(SocketGuild guild);
}
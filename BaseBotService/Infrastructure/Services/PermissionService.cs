using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using Discord.WebSocket;

namespace BaseBotService.Infrastructure.Services;
public class PermissionService : IPermissionService
{
    private readonly ILogger _logger;
    private readonly IGuildRepository _guildRepo;

    public PermissionService(ILogger logger, IGuildRepository guildRepo)
    {
        _logger = logger;
        _guildRepo = guildRepo;
    }

    public bool IsUserAdmin(SocketGuildUser user) => user.GuildPermissions.Administrator;

    public bool IsUserModerator(SocketGuildUser user, SocketRole modRoles) => user.Roles.Contains(modRoles);

    public List<ulong> GetModeratorRoles(SocketGuild guild) => _guildRepo.GetGuild(guild.Id, true).ModeratorRoles;

    public bool CanUserExecuteCommandAsync(SocketGuildUser user, SocketGuild guild)
    {
        List<ulong> moderatorRoles = GetModeratorRoles(guild);
        return IsUserAdmin(user) || user.Roles.Any(role => moderatorRoles.Contains(role.Id));
    }

    public void SetModeratorRole(SocketGuild guild, SocketRole role)
    {
        var currentGuild = _guildRepo.GetGuild(guild.Id, true);
        currentGuild.ModeratorRoles.Clear();
        currentGuild.ModeratorRoles.Add(role.Id);
        _guildRepo.UpdateGuild(currentGuild);
    }
}

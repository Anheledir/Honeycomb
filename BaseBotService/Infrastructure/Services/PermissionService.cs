using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using Discord.WebSocket;
using System.Collections.Concurrent;

namespace BaseBotService.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly ILogger _logger;
    private readonly IGuildRepository _guildRepo;
    private readonly ConcurrentDictionary<ulong, List<ulong>> _moderatorRoleCache = new();
    private readonly ConcurrentDictionary<ulong, List<ulong>> _artistRoleCache = new();

    public PermissionService(ILogger logger, IGuildRepository guildRepo)
    {
        _logger = logger;
        _guildRepo = guildRepo;
    }

    public bool IsUserAdmin(SocketGuildUser user) => user.GuildPermissions.Administrator;

    public bool IsUserModerator(SocketGuildUser user, SocketRole modRole) => user.Roles.Contains(modRole);

    public async Task<List<ulong>> GetModeratorRolesAsync(SocketGuild guild)
    {
        if (_moderatorRoleCache.TryGetValue(guild.Id, out var cachedRoles))
        {
            return cachedRoles;
        }

        var guildData = await _guildRepo.GetGuildAsync(guild.Id, true);
        _moderatorRoleCache[guild.Id] = guildData.ModeratorRoles;
        return guildData.ModeratorRoles;
    }

    public async Task<List<ulong>> GetArtistRolesAsync(SocketGuild guild)
    {
        if (_artistRoleCache.TryGetValue(guild.Id, out var cachedRoles))
        {
            return cachedRoles;
        }

        var guildData = await _guildRepo.GetGuildAsync(guild.Id, true);
        _artistRoleCache[guild.Id] = guildData.ArtistRoles;
        return guildData.ArtistRoles;
    }

    public async Task<bool> CanUserExecuteModeratorCommandAsync(SocketGuildUser? user)
    {
        if (user == null) return false;
        var moderatorRoles = await GetModeratorRolesAsync(user.Guild);
        return IsUserAdmin(user) || user.Roles.Any(role => moderatorRoles.Contains(role.Id));
    }

    public async Task<bool> CanUserExecuteArtistCommandAsync(SocketGuildUser? user)
    {
        if (user == null) return false;
        var artistRoles = await GetArtistRolesAsync(user.Guild);
        return user.Roles.Any(role => artistRoles.Contains(role.Id));
    }

    public async Task SetModeratorRoleAsync(SocketGuild guild, SocketRole role)
    {
        var currentGuild = await _guildRepo.GetGuildAsync(guild.Id, true);
        currentGuild.ModeratorRoles.Clear();
        currentGuild.ModeratorRoles.Add(role.Id);
        _moderatorRoleCache[guild.Id] = currentGuild.ModeratorRoles;
        await _guildRepo.UpdateGuildAsync(currentGuild);
    }

    public async Task<SocketRole?> GetModeratorRoleAsync(SocketGuild guild)
    {
        var currentGuild = await _guildRepo.GetGuildAsync(guild.Id, true);
        return currentGuild.ModeratorRoles.Count > 0 ? guild.GetRole(currentGuild.ModeratorRoles[0]) : null;
    }

    public void InvalidateCacheForGuild(ulong guildId)
    {
        _moderatorRoleCache.TryRemove(guildId, out _);
        _artistRoleCache.TryRemove(guildId, out _);
    }
}

using Discord.WebSocket;

namespace BaseBotService.Core.Interfaces;

/// <summary>
/// Provides methods for handling permissions within a Discord guild.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Asynchronously determines whether a user can execute a moderator command.
    /// </summary>
    /// <param name="user">The user to check permissions for.</param>
    /// <returns>A Task representing the asynchronous operation, with a boolean indicating whether the user can execute a moderator command.</returns>
    Task<bool> CanUserExecuteModeratorCommandAsync(SocketGuildUser? user);

    /// <summary>
    /// Asynchronously determines whether a user can execute an artist command.
    /// </summary>
    /// <param name="user">The user to check permissions for.</param>
    /// <returns>A Task representing the asynchronous operation, with a boolean indicating whether the user can execute an artist command.</returns>
    Task<bool> CanUserExecuteArtistCommandAsync(SocketGuildUser? user);

    /// <summary>
    /// Asynchronously gets the list of moderator role IDs for a guild.
    /// </summary>
    /// <param name="guild">The guild to retrieve moderator roles for.</param>
    /// <returns>A Task representing the asynchronous operation, with a list of role IDs that are considered moderator roles in the guild.</returns>
    Task<List<ulong>> GetModeratorRolesAsync(SocketGuild guild);

    /// <summary>
    /// Asynchronously gets the list of artist role IDs for a guild.
    /// </summary>
    /// <param name="guild">The guild to retrieve artist roles for.</param>
    /// <returns>A Task representing the asynchronous operation, with a list of role IDs that are considered artist roles in the guild.</returns>
    Task<List<ulong>> GetArtistRolesAsync(SocketGuild guild);

    /// <summary>
    /// Determines whether a user is an administrator.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <returns>True if the user has administrative permissions; otherwise, false.</returns>
    bool IsUserAdmin(SocketGuildUser user);

    /// <summary>
    /// Determines whether a user has a specific moderator role.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <param name="modRole">The moderator role to check against.</param>
    /// <returns>True if the user has the specified moderator role; otherwise, false.</returns>
    bool IsUserModerator(SocketGuildUser user, SocketRole modRole);

    /// <summary>
    /// Asynchronously sets a role as the moderator role for a guild.
    /// </summary>
    /// <param name="guild">The guild to set the moderator role for.</param>
    /// <param name="role">The role to set as the moderator role.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task SetModeratorRoleAsync(SocketGuild guild, SocketRole role);

    /// <summary>
    /// Asynchronously retrieves the moderator role for a guild.
    /// </summary>
    /// <param name="guild">The guild to retrieve the moderator role for.</param>
    /// <returns>A Task representing the asynchronous operation, with the moderator role of the guild, or null if no role is set.</returns>
    Task<SocketRole?> GetModeratorRoleAsync(SocketGuild guild);

    /// <summary>
    /// Invalidates the cached roles for a specific guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    void InvalidateCacheForGuild(ulong guildId);
}

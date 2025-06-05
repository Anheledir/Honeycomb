using BaseBotService.Data.Models;

namespace BaseBotService.Data.Interfaces;

/// <summary>
/// Represents a repository for managing GuildMemberHC entities.
/// </summary>
public interface IGuildMemberRepository
{
    /// <summary>
    /// Retrieves a GuildMemberHC entity for the specified user in the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The GuildMemberHC entity if found, otherwise null.</returns>
    GuildMemberHC GetUser(ulong guildId, ulong userId);

    /// <summary>
    /// Adds a new GuildMemberHC entity to the repository.
    /// </summary>
    /// <param name="user">The GuildMemberHC entity to be added.</param>
    void AddUser(GuildMemberHC user);

    /// <summary>
    /// Updates an existing GuildMemberHC entity in the repository.
    /// </summary>
    /// <param name="user">The GuildMemberHC entity to be updated.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
    bool UpdateUser(GuildMemberHC user);

    /// <summary>
    /// Deletes a GuildMemberHC entity for the specified user in the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>True if the delete was successful, otherwise false.</returns>
    bool DeleteUser(ulong guildId, ulong userId);
    int DeleteGuild(ulong guildId);

    /// <summary>
    /// Gets the top users of a guild ordered by their activity points.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="limit">The maximum number of users to return.</param>
    /// <returns>An enumerable of <see cref="GuildMemberHC"/> ordered by activity points.</returns>
    IEnumerable<GuildMemberHC> GetTopUsers(ulong guildId, int limit);
}
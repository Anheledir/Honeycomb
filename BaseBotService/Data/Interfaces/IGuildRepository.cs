using BaseBotService.Data.Models;

namespace BaseBotService.Data.Interfaces;

/// <summary>
/// Represents a repository for managing GuildHC objects.
/// </summary>
public interface IGuildRepository
{
    /// <summary>
    /// Retrieves a GuildHC object by its guild ID.
    /// </summary>
    /// <param name="guildId">The guild ID.</param>
    /// <param name="create">If true, it will create a new guild and return it in case no user with <paramref name="guildId"/> exists. Default is false.</param>
    /// <returns>The GuildHC object with the specified guild ID, or null if not found. If <paramref name="create"/> is true will always return an entity.</returns>
    GuildHC? GetGuild(ulong guildId, bool create = false);

    /// <summary>
    /// Adds a new GuildHC object to the repository.
    /// </summary>
    /// <param name="guild">The GuildHC object to add.</param>
    void AddGuild(GuildHC guild);

    /// <summary>
    /// Updates an existing GuildHC object in the repository.
    /// </summary>
    /// <param name="guild">The GuildHC object to update.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    bool UpdateGuild(GuildHC guild);

    /// <summary>
    /// Deletes a GuildHC object from the repository by its guild ID.
    /// </summary>
    /// <param name="guildId">The guild ID.</param>
    /// <returns>True if the deletion was successful, false otherwise.</returns>
    bool DeleteGuild(ulong guildId);
}

using BaseBotService.Data.Models;

namespace BaseBotService.Data.Interfaces;

/// <summary>
/// Interface for the GuildHC repository, providing CRUD operations for GuildHC entities.
/// </summary>
public interface IGuildRepository
{
    /// <summary>
    /// Asynchronously retrieves a GuildHC entity by its guild ID.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="create">If true, creates and returns a new GuildHC entity if none exists with the specified <paramref name="guildId"/>. Default is false.</param>
    /// <returns>A Task that represents the asynchronous operation, with a GuildHC entity if found; otherwise, null. If <paramref name="create"/> is true, always returns an entity.</returns>
    Task<GuildHC?> GetGuildAsync(ulong guildId, bool create = false);

    /// <summary>
    /// Asynchronously adds a new GuildHC entity to the repository.
    /// </summary>
    /// <param name="guild">The GuildHC entity to add.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    Task AddGuildAsync(GuildHC guild);

    /// <summary>
    /// Asynchronously updates an existing GuildHC entity in the repository.
    /// </summary>
    /// <param name="guild">The GuildHC entity to update.</param>
    /// <returns>A Task that represents the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> UpdateGuildAsync(GuildHC guild);

    /// <summary>
    /// Asynchronously deletes a GuildHC entity by its guild ID.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <returns>A Task that represents the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> DeleteGuildAsync(ulong guildId);
}

using BaseBotService.Data.Models;

namespace BaseBotService.Data.Interfaces;

/// <summary>
/// Represents a repository for managing GuildMemberHC entities.
/// </summary>
public interface IGuildMemberRepository
{
    /// <summary>
    /// Asynchronously retrieves a GuildMemberHC entity for the specified user in the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A Task representing the asynchronous operation, with a GuildMemberHC entity if found, otherwise null.</returns>
    Task<GuildMemberHC?> GetUserAsync(ulong guildId, ulong userId);

    /// <summary>
    /// Asynchronously adds a new GuildMemberHC entity to the repository.
    /// </summary>
    /// <param name="user">The GuildMemberHC entity to be added.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task AddUserAsync(GuildMemberHC user);

    /// <summary>
    /// Asynchronously updates an existing GuildMemberHC entity in the repository.
    /// </summary>
    /// <param name="user">The GuildMemberHC entity to be updated.</param>
    /// <returns>A Task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> UpdateUserAsync(GuildMemberHC user);

    /// <summary>
    /// Asynchronously deletes a GuildMemberHC entity for the specified user in the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A Task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> DeleteUserAsync(ulong guildId, ulong userId);

    /// <summary>
    /// Asynchronously deletes all GuildMemberHC entities for the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <returns>A Task representing the asynchronous operation, with an integer indicating the number of entities deleted.</returns>
    Task<int> DeleteGuildAsync(ulong guildId);
}

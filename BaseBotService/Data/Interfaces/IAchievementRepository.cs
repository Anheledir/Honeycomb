using BaseBotService.Core.Base;

namespace BaseBotService.Data.Interfaces;

/// <summary>
/// Represents a repository for managing achievements.
/// </summary>
/// <typeparam name="T">The type of the achievement derived from AchievementBase.</typeparam>
public interface IAchievementRepository<T> where T : AchievementBase
{
    Guid Identifier { get; }

    /// <summary>
    /// Retrieves all achievements.
    /// </summary>
    /// <returns>A list of all achievements.</returns>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Inserts a new achievement into the repository.
    /// </summary>
    /// <param name="entity">The achievement to insert.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> InsertAsync(T entity);

    /// <summary>
    /// Updates an existing achievement in the repository.
    /// </summary>
    /// <param name="entity">The achievement to update.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    Task<bool> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an achievement from the repository.
    /// </summary>
    /// <param name="id">The Guid of the achievement to delete.</param>
    /// <returns>True if the delete was successful, false otherwise.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Retrieves achievements for a specific user and guild.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="guildId">The guild's ID.</param>
    /// <returns>A list of achievements.</returns>
    Task<List<T>> GetAsync(ulong userId, ulong guildId);

    /// <summary>
    /// Retrieves achievements for a specific guild.
    /// </summary>
    /// <param name="guildId">The guild's ID.</param>
    /// <returns>A list of achievements.</returns>
    Task<List<T>> GetByGuildIdAsync(ulong guildId);

    /// <summary>
    /// Retrieves achievements for a specific user.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>A list of achievements.</returns>
    Task<List<T>> GetByUserIdAsync(ulong userId);
}

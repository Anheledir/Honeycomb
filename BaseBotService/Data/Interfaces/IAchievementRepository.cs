using BaseBotService.Core.Base;
using LiteDB;

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
    /// <returns>An IEnumerable of all achievements.</returns>
    List<T> GetAll();

    /// <summary>
    /// Inserts a new achievement into the repository.
    /// </summary>
    /// <param name="entity">The achievement to insert.</param>
    /// <returns>The ObjectId of the inserted achievement.</returns>
    ObjectId Insert(T entity);

    /// <summary>
    /// Updates an existing achievement in the repository.
    /// </summary>
    /// <param name="entity">The achievement to update.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    bool Update(T entity);

    /// <summary>
    /// Deletes an achievement from the repository.
    /// </summary>
    /// <param name="id">The ObjectId of the achievement to delete.</param>
    /// <returns>True if the delete was successful, false otherwise.</returns>
    bool Delete(ObjectId id);
    List<T> Get(ulong userId, ulong guildId);
    List<T> GetByGuildId(ulong guildId);
    List<T> GetByUserId(ulong userId);
}
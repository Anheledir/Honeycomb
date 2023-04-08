using BaseBotService.Core.Base;
using BaseBotService.Data.Interfaces;
using LiteDB;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BaseBotService.Data.Repositories;
public class AchievementRepository<T> : IAchievementRepository<T> where T : AchievementBase
{
    private readonly ILiteCollection<AchievementBase> _achievements;
    private readonly Guid _identifier = GetIdentifier();

    public AchievementRepository(ILiteCollection<AchievementBase> achievements) => _achievements = achievements;

    public List<T> GetByUserId(ulong userId)
        => _achievements.Find(a => a.SourceIdentifier == _identifier && a.MemberId == userId).OfType<T>().ToList();

    public List<T> GetByGuildId(ulong guildId)
        => _achievements.Find(a => a.SourceIdentifier == _identifier && a.GuildId == guildId).OfType<T>().ToList();

    public List<T> Get(ulong userId, ulong guildId)
        => _achievements.Find(a => a.SourceIdentifier == _identifier && a.MemberId == userId && a.GuildId == guildId).OfType<T>().ToList();

    /// <summary>
    /// Retrieves all achievements of type T with the same Identifier value as the T type.
    /// </summary>
    /// <returns>An IEnumerable of achievements of type T with the specified Identifier value.</returns>
    public List<T> GetAll()
    {
        // Filter the achievements based on the Identifier property value
        return _achievements.Find(a => a.SourceIdentifier == _identifier).OfType<T>().ToList();
    }

    [SuppressMessage("Roslynator", "RCS1158:Static member in generic type should use a type parameter.", Justification = "Only used internally.")]
    internal static Guid GetIdentifier()
    {
        // Get the Identifier property value for the T type
        const string propertyName = nameof(AchievementBase.Identifier);
        PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public)!;

        // By passing null, we are telling the reflection API to get the value of the static property
        // without referring to any specific instance of the type.
        return new Guid((string)propertyInfo.GetValue(null)!);
    }

    /// <summary>
    /// Inserts a new achievement of type T into the LiteDB collection.
    /// </summary>
    /// <param name="entity">The achievement of type T to insert.</param>
    /// <returns>The ObjectId of the inserted achievement.</returns>
    public ObjectId Insert(T entity) => _achievements.Insert(entity);

    /// <summary>
    /// Updates an existing achievement of type T in the LiteDB collection.
    /// </summary>
    /// <param name="entity">The achievement of type T to update.</param>
    /// <returns>true if the achievement was updated successfully; otherwise, false.</returns>
    public bool Update(T entity) => _achievements.Update(entity);

    /// <summary>
    /// Deletes an achievement of type T from the LiteDB collection by its ObjectId.
    /// </summary>
    /// <param name="id">The ObjectId of the achievement to delete.</param>
    /// <returns>true if the achievement was deleted successfully; otherwise, false.</returns>
    public bool Delete(ObjectId id) => _achievements.Delete(id);
}
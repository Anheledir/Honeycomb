using LiteDB;

namespace BaseBotService.Data.Interfaces;

/// <summary>
/// Represents a persistence service for managing LiteDB collections.
/// </summary>
public interface IPersistenceService : IDisposable
{
    /// <summary>
    /// Gets a LiteDB collection of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <returns>A LiteDB collection of the specified type.</returns>
    ILiteCollection<T> GetCollection<T>();
}

using BaseBotService.Data.Interfaces;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Core.Base;

/// <summary>
/// Serves as the base model for all data entities, providing common properties and methods.
/// </summary>
public abstract class ModelBase
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.NewObjectId();

    /// <summary>
    /// Returns a LiteDB collection of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the collection.</typeparam>
    /// <param name="services">The service provider to get services from.</param>
    /// <returns>A LiteDB collection of the specified type.</returns>
    public static ILiteCollection<T> GetServiceRegistration<T>(IServiceProvider services)
        => services.GetRequiredService<IPersistenceService>().GetCollection<T>();
}
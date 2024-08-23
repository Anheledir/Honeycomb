using BaseBotService.Data.Interfaces;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Core.Base;

public abstract class ModelBase
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.NewObjectId();

    public static ILiteCollection<T> GetServiceRegistration<T>(IServiceProvider services) where T : ModelBase
    {
        // Retrieve the collection from the PersistenceService
        ILiteCollection<T> collection = services.GetRequiredService<IPersistenceService>().GetCollection<T>();

        // Create an instance of the model to call EnsureIndexes
        var modelInstance = ActivatorUtilities.CreateInstance<T>(services);
        modelInstance.EnsureIndexes(collection);

        return collection;
    }

    protected virtual void EnsureIndexes<T>(ILiteCollection<T> collection)
    {
        // Default implementation does nothing; override in derived classes to set up indexes
    }
}

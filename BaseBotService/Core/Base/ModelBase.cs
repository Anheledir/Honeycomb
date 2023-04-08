using BaseBotService.Data.Interfaces;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Core.Base;

public abstract class ModelBase
{
    [BsonId]
    public ObjectId Id { get; set; } = null!;

    public static ILiteCollection<T> GetServiceRegistration<T>(IServiceProvider services)
        => services.GetRequiredService<IPersistenceService>().GetCollection<T>();
}
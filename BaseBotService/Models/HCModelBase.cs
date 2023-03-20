using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Models;

public abstract class HCModelBase
{
    [BsonId]
    public ObjectId Id { get; set; } = null!;

    public static ILiteCollection<T> GetServiceRegistration<T>(IServiceProvider services)
    {
        return services.GetService<IPersistenceService>().GetCollection<T>();
    }
}
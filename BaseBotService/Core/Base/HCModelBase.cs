using BaseBotService.Core.Interfaces;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Core.Base;

public abstract class HCModelBase
{
    [BsonId]
    public ObjectId Id { get; set; } = null!;

    public static ILiteCollection<T> GetServiceRegistration<T>(IServiceProvider services) => services.GetService<IPersistenceService>().GetCollection<T>();
}
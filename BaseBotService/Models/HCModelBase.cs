using BaseBotService.Interfaces;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Models;

public class HCModelBase
{
    public static ILiteCollection<T> GetServiceRegistration<T>(IServiceProvider services)
    {
        return services.GetService<IPersistenceService>().GetCollection<T>();
    }
}
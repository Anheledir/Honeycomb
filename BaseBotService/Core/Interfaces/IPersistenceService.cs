using LiteDB;

namespace BaseBotService.Core.Interfaces;
public interface IPersistenceService : IDisposable
{
    ILiteCollection<T> GetCollection<T>();
}
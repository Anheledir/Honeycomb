using LiteDB;

namespace BaseBotService.Interfaces;
public interface IPersistenceService : IDisposable
{
    ILiteCollection<T> GetCollection<T>();
}
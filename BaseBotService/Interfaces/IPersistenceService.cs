using LiteDB;

namespace BaseBotService.Interfaces;
public interface IPersistenceService : IDisposable
{
    void Commit();
    ILiteCollection<T> GetCollection<T>();
}
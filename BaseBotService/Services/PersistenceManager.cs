using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using LiteDB;
using Serilog;

namespace BaseBotService.Services;

public class PersistenceService : IPersistenceService
{
    private readonly LiteDatabase _database;
    private readonly ILogger _logger;
    private bool disposedValue;

    public PersistenceService(ILogger logger, IEnvironmentService environment)
    {
        string connectionString = $"Filename={environment.DatabaseFile};";

        if (environment.UseAzureStorage)
        {
            connectionString += $"Mode=azure;AccountName={environment.AzureStorageAccount};AccountKey={environment.AzureStorageKey};Container={environment.AzureStorageContainer};";
        }

        _logger.Information($"LiteDB connection string: {connectionString.Replace(environment.AzureStorageKey!, environment.AzureStorageKey!.MaskToken())}");
        _database = new LiteDatabase(connectionString);
        _logger = logger;
    }

    public ILiteCollection<T> GetCollection<T>()
    {
        _logger.Debug($"GetCollection<{nameof(T)}>()");
        return _database.GetCollection<T>();
    }

    public void Commit()
    {
        _logger.Information("Commit transaction.");
        _database.Commit();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _database.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

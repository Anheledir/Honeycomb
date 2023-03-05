using BaseBotService.Interfaces;
using LiteDB;
using Serilog;

namespace BaseBotService.Services;

public class PersistenceService : IPersistenceService
{
    private readonly LiteDatabase _database;
    private readonly ILogger _logger;
    private readonly Timer _timer;
    private readonly object _lock = new object();
    private bool disposedValue;

    public PersistenceService(ILogger logger, IEnvironmentService environment)
    {
        _logger = logger;

        string connectionString = $"Filename={environment.DatabaseFile};";
        _logger.Information($"LiteDB connection string: {connectionString}");
        _database = new LiteDatabase(connectionString);

        // Set up the timer to trigger every minute
        _timer = new Timer(HandleAutoSaveTimer);
        _timer.Change(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    private void HandleAutoSaveTimer(object? state) => Commit();

    public ILiteCollection<T> GetCollection<T>()
    {
        _logger.Debug($"GetCollection<{nameof(T)}>()");
        return _database.GetCollection<T>();
    }

    private void Commit()
    {
        lock (_lock)
        {
            _logger.Debug("Commit transaction to database file.");
            _database.Commit();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _timer.Dispose();
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

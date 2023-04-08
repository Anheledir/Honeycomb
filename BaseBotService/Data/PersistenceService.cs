using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using BaseBotService.Utilities.Extensions;
using LiteDB;

namespace BaseBotService.Data;

public class PersistenceService : IPersistenceService
{
    private readonly LiteDatabase _database;
    private readonly ILogger _logger;
    private Timer _timer = null!;
    private readonly object _lock = new();
    private bool _disposedValue;
    private readonly TimeSpan _savingInterval = TimeSpan.FromMinutes(1);
    private readonly TimeSpan _checkpointInterval = TimeSpan.FromDays(1);
    private bool _isReady;
    private DateTime _lastCheckpoint = DateTime.UtcNow;

    public PersistenceService(ILogger logger, MigrationManager migrationManager, IEnvironmentService environment)
    {
        _logger = logger.ForContext<PersistenceService>();

        var connection = new ConnectionString(environment.ConnectionString);
        _logger.Debug("Loading LightDB with '{@connection}'", connection);
        BsonMapper mapper = new();
        _database = new LiteDatabase(connection, mapper);
        _database.Checkpoint();
        Task.Run(async () =>
        {
            _logger.Debug($"Checking for database migrations available for version {_database.UserVersion}.");
            if (migrationManager.AreMigrationsAvailable(_database))
            {
                _logger.Information("The database must get migrated.");
                _logger.Debug($"Creating backup of database at '{connection.Filename}'.");
                string backupPath = await _database.BackupDatabaseAsync(connection.Filename);
                _logger.Information($"Database backup was created at '{backupPath}'.");

                _logger.Debug("Applying database migrations.");
                if (!migrationManager.ApplyMigrations(_database))
                {
                    _logger.Fatal("Database migration couldn't be completed.");
                    throw new OperationCanceledException();
                }
                _logger.Information("Database migration finished.");
            }

            _logger.Debug("Registering database collections and relations in-between them.");
            CollectionMapper.RegisterCollections(ref mapper);
            _logger.Information($"Starting the database save timer, interval is {_savingInterval.TotalSeconds} seconds.");
            _timer = new Timer(HandleAutoSaveTimer);
            _ = _timer.Change(TimeSpan.FromMinutes(1), _savingInterval);
            _isReady = true;
        });
    }

    private void HandleAutoSaveTimer(object? state) => Commit();

    public ILiteCollection<T> GetCollection<T>()
    {
        while (!_isReady)
        {
            Thread.Sleep(100);
        }

        _logger.Debug($"GetCollection<{typeof(T)}>()");
        return _database.GetCollection<T>();
    }

    private void Commit()
    {
        lock (_lock)
        {
            _logger.Debug("Commit transaction to database file.");
            _ = _database.Commit();

            if (DateTime.UtcNow - _lastCheckpoint > _checkpointInterval)
            {
                _logger.Debug("Creating database checkpoint.");
                _database.Checkpoint();
                _lastCheckpoint = DateTime.UtcNow;
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _timer.Dispose();
                _database.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

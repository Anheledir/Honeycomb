using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using BaseBotService.Utilities.Extensions;
using LiteDB;

namespace BaseBotService.Data;

public class PersistenceService : IPersistenceService
{
    private readonly LiteDatabase _database;
    private readonly ILogger _logger;
    private Timer _timer;
    private readonly object _lock = new();
    private bool _disposedValue;
    private readonly TimeSpan _savingInterval = TimeSpan.FromMinutes(1);
    private readonly TimeSpan _checkpointInterval = TimeSpan.FromDays(1);
    private DateTime _lastCheckpoint = DateTime.UtcNow;

    public PersistenceService(ILogger logger, MigrationManager migrationManager, IEnvironmentService environment, CancellationTokenSource cts, CollectionMapper collectionMapper)
    {
        _logger = logger.ForContext<PersistenceService>();

        var connection = new ConnectionString(environment.ConnectionString);
        _logger.Debug("Loading LiteDB with '{@connection}'", connection);
        BsonMapper mapper = new();
        collectionMapper.RegisterCollections(ref mapper);
        _database = new LiteDatabase(connection, mapper)
        {
            UtcDate = true
        };

        InitializeDatabase(migrationManager, connection, cts).ConfigureAwait(false).GetAwaiter().GetResult();

        _timer = new Timer(HandleAutoSaveTimer, null, TimeSpan.FromMinutes(1), _savingInterval);
    }

    private async Task InitializeDatabase(MigrationManager migrationManager, ConnectionString connection, CancellationTokenSource cts)
    {
        _database.Checkpoint();

        try
        {
            _logger.Debug($"Checking for database migrations available for version {_database.UserVersion}.");
            if (migrationManager.AreMigrationsAvailable(_database))
            {
                _logger.Information("The database must be migrated.");
                _logger.Debug($"Creating backup of database at '{connection.Filename}'.");
                string backupPath = await _database.BackupDatabaseAsync(connection.Filename);
                _logger.Information($"Database backup was created at '{backupPath}'.");

                _logger.Debug("Applying database migrations.");
                if (!migrationManager.ApplyMigrations(_database))
                {
                    _logger.Fatal("Database migration couldn't be completed.");
                    cts.Cancel();
                }
                _logger.Information("Database migration finished.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred during the database initialization.");
            throw;
        }

        _logger.Debug("Registering database collections and relations in-between them.");
        _logger.Information($"Database save timer started with an interval of {_savingInterval.TotalSeconds} seconds.");
    }

    private void HandleAutoSaveTimer(object? state) => Commit();

    public ILiteCollection<T> GetCollection<T>()
    {
        _logger.Debug($"GetCollection<{typeof(T)}>()");
        if (_disposedValue)
            throw new ObjectDisposedException(nameof(PersistenceService));

        return _database.GetCollection<T>();
    }

    private void Commit()
    {
        lock (_lock)
        {
            if (_disposedValue) return;

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
                lock (_lock)
                {
                    _timer?.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
                    _timer?.Dispose();
                    _database?.Dispose();
                }
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
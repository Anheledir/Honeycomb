using LiteDB;

namespace BaseBotService.Data;
public class MigrationManager
{
    private readonly int _currentDatabaseVersion = 0;
    private readonly ILogger _logger;

    public MigrationManager(ILogger logger)
    {
        _logger = logger.ForContext<MigrationManager>();
    }

    public bool AreMigrationsAvailable(ILiteDatabase database) => database != null && database.UserVersion < _currentDatabaseVersion;

    public async Task<bool> ApplyMigrations(ILiteDatabase database)
    {
        bool success = true;
        while (success && database.UserVersion < _currentDatabaseVersion)
        {
            success = await Task.Run(() => ApplyChanges(database));
        }
        return success;
    }

    private bool ApplyChanges(ILiteDatabase db)
    {
        int version = db.UserVersion;
        _logger.Information($"Looking for database migrations for version {version}");
        db.BeginTrans();
        try
        {
            switch (version)
            {
                case 0:
                    _logger.Debug($"Applying database migrations for version {version}");
                    // Add necessary changes to the database
                    break;
                default:
                    _logger.Debug($"No database migrations found for version {version}");
                    break;
            }
            db.UserVersion++;
            _logger.Information($"New database version is {db.UserVersion}, committing changes.");
            db.Commit();
            return true;
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, $"The database migration for version {version} failed, doing rollback!");
            db.Rollback();
            return false;
        }
    }
}

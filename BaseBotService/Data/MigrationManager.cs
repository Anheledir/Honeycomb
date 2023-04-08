using BaseBotService.Data.Interfaces;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Data;
public class MigrationManager
{
    private readonly int _targetDatabaseVersion;
    private readonly ILogger _logger;
    private readonly IServiceProvider _service;

    public MigrationManager(ILogger logger, IServiceProvider service, int targetDatabaseVersion = 1)
    {
        _logger = logger.ForContext<MigrationManager>();
        _service = service;
        _targetDatabaseVersion = targetDatabaseVersion;
    }

    public bool AreMigrationsAvailable(ILiteDatabase database) => database != null && database.UserVersion < _targetDatabaseVersion;

    public bool ApplyMigrations(ILiteDatabase database)
    {
        bool success = true;
        while (success && database.UserVersion < _targetDatabaseVersion)
        {
            success = ApplyChanges(database);
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
            // Get all registered IMigrationChangeset instances
            var migrationChangesets = _service.GetServices<IMigrationChangeset>();

            // Find the first Changeset with the matching version
            var matchingChangeset = migrationChangesets.FirstOrDefault(changeset => changeset.Version == version);

            if (matchingChangeset != null)
            {
                _logger.Information($"Applying database migrations for version {version}");
                if (!matchingChangeset.ApplyChangeset(db, version))
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                _logger.Debug($"No database migrations found for version {version}");
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
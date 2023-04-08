using BaseBotService.Data.Interfaces;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Data;
public class MigrationManager
{
    private readonly int _targetDatabaseVersion;
    private readonly ILogger _logger;
    private readonly IServiceProvider _service = Program.ServiceProvider;

    public MigrationManager()
    {
        _logger = _service.GetRequiredService<ILogger>().ForContext<MigrationManager>();
        _targetDatabaseVersion = 1;
    }

    public bool AreMigrationsAvailable(ILiteDatabase database) => database != null && database.UserVersion < _targetDatabaseVersion;

    public bool ApplyMigrations(ILiteDatabase database)
    {
        bool success = true;
        database.BeginTrans();

        while (success && database.UserVersion < _targetDatabaseVersion)
        {
            success = ApplyChanges(database);
        }

        if (success)
        {
            database.Commit();
            database.Checkpoint();
        }
        else
        {
            database.Rollback();
        }
        return success;
    }

    private bool ApplyChanges(ILiteDatabase db)
    {
        int version = db.UserVersion;
        _logger.Information($"Looking for database migrations for version {version}");

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

            try
            {
                db.Commit();
                db.UserVersion++;
                _logger.Information($"New database version is {db.UserVersion}, committing changes.");
                db.Commit();
                return true;
            }
            catch
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, $"The database migration for version {version} failed, doing rollback!");
            return false;
        }
    }
}
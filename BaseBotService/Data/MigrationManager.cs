using BaseBotService.Data.Interfaces;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Data
{
    public class MigrationManager
    {
        private readonly int _targetDatabaseVersion;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public MigrationManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger>().ForContext<MigrationManager>();
            _targetDatabaseVersion = 1;
        }

        public bool AreMigrationsAvailable(ILiteDatabase database) => database != null && database.UserVersion < _targetDatabaseVersion;

        public bool ApplyMigrations(ILiteDatabase database)
        {
            bool success = true;
            _logger.Information($"Starting database migration to version {_targetDatabaseVersion}.");

            lock (database) // Ensure single-threaded access
            {
                try
                {
                    database.BeginTrans();

                    while (success && database.UserVersion < _targetDatabaseVersion)
                    {
                        success = ApplyChanges(database);
                    }

                    if (success)
                    {
                        database.Commit();
                        database.Checkpoint();
                        _logger.Information("Database migration completed successfully.");
                    }
                    else
                    {
                        database.Rollback();
                        _logger.Error("Database migration failed, rolled back to previous state.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex, "Unexpected error during database migration. Rolling back.");
                    database.Rollback();
                    success = false;
                }
            }

            return success;
        }

        public bool ApplyChanges(ILiteDatabase db)
        {
            int version = db.UserVersion;
            _logger.Information($"Looking for database migrations for version {version}");

            try
            {
                var migrationChangesets = _serviceProvider.GetServices<IMigrationChangeset>().ToList();
                _logger.Debug($"Total migration changesets registered: {migrationChangesets.Count}");

                if (!migrationChangesets.Any())
                {
                    _logger.Error("No migration changesets found. Ensure that migrations are registered properly.");
                    return false;
                }

                var matchingChangeset = migrationChangesets.FirstOrDefault(changeset => changeset.Version == version);

                if (matchingChangeset != null)
                {
                    _logger.Information($"Applying database migrations for version {version} using changeset '{matchingChangeset.GetType().Name}'.");

                    bool result = matchingChangeset.ApplyChangeset(db, version);

                    if (!result)
                    {
                        _logger.Error($"Migration changeset for version {version} failed to apply.");
                        return false;
                    }

                    db.UserVersion++;
                    _logger.Information($"Database successfully migrated to version {db.UserVersion}.");
                }
                else
                {
                    _logger.Warning($"No migration changeset found for version {version}. Skipping migration.");
                    db.UserVersion = _targetDatabaseVersion;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"An exception occurred while applying migration for version {version}.");
                return false;
            }
        }
    }
}

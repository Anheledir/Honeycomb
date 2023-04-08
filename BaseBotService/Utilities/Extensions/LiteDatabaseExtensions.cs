using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Utilities.Extensions;
public static class LiteDatabaseExtensions
{
    public static async Task<string> BackupDatabaseAsync(this ILiteDatabase database, string filename)
    {
        ILogger logger = Program.ServiceProvider.GetRequiredService<ILogger>();

        logger.Debug("BackupDatabaseAsync: Checkpoint to flush all dirty pages to disk.");
        database.Checkpoint();

        logger.Debug("BackupDatabaseAsync: Rebuild all Database to remove unused pages.");
        database.Rebuild();

        using var sourceStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 4096, useAsync: true);
        var backupFilePath = $"{filename}.{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.bak";

        logger.Debug($"BackupDatabaseAsync: Copy the original Database file '{filename}' to the backup file '{backupFilePath}'.");

        using var destinationStream = new FileStream(backupFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
        await sourceStream.CopyToAsync(destinationStream);

        logger.Debug($"Backup database: {backupFilePath}");
        return backupFilePath;
    }
}

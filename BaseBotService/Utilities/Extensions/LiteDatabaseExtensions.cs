using LiteDB;

namespace BaseBotService.Utilities.Extensions;
public static class LiteDatabaseExtensions
{
    public static async Task<string> BackupDatabaseAsync(this ILiteDatabase database, string filename)
    {
        // Checkpoint to flush all dirty pages to disk
        database.Checkpoint();

        // Copy the original file to the backup file using async file operations
        using var sourceStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 4096, useAsync: true);
        // create a new file name with the current date and time
        var backupFilePath = $"{filename}.{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.bak";
        using var destinationStream = new FileStream(backupFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
        await sourceStream.CopyToAsync(destinationStream);
        return backupFilePath;
    }
}

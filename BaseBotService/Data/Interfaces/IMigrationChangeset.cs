using LiteDB;

namespace BaseBotService.Data.Interfaces;
public interface IMigrationChangeset
{
    /// <summary>
    /// What database version this changeset can be applied to.
    /// </summary>
    public int Version { get; }

    /// <summary>
    /// Applies the changeset to the database.
    /// </summary>
    /// <param name="db">The LiteDB database.</param>
    /// <returns></returns>
    public Task<bool> ApplyChangeset(ILiteDatabase db, int dbVersion);
}

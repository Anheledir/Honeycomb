namespace BaseBotService.Commands.Interfaces;

/// <summary>
/// Interface for managing user engagement within a guild, tracking activity points and last active time.
/// </summary>
public interface IEngagementService
{
    /// <summary>
    /// Asynchronously adds an activity tick for the specified user in the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    Task AddActivityTickAsync(ulong guildId, ulong userId);

    /// <summary>
    /// Asynchronously retrieves the activity points for a specified user in the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A Task representing the asynchronous operation, containing the number of activity points the user has in the guild.</returns>
    Task<uint> GetActivityPointsAsync(ulong guildId, ulong userId);

    /// <summary>
    /// Asynchronously retrieves the last active date and time for a specified user in the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A Task representing the asynchronous operation, containing the date and time when the user was last active in the guild.</returns>
    Task<DateTime> GetLastActiveAsync(ulong guildId, ulong userId);

    /// <summary>
    /// Gets the maximum number of activity points a user can earn per day.
    /// </summary>
    int MaxPointsPerDay { get; }
}

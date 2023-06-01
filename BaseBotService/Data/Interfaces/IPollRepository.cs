using BaseBotService.Data.Models;

namespace BaseBotService.Data.Interfaces;
/// <summary>
/// Represents the Poll repository interface.
/// </summary>
public interface IPollRepository
{
    /// <summary>
    /// Adds a poll to the repository.
    /// </summary>
    /// <param name="poll">The poll to add.</param>
    void AddPoll(PollHC poll);

    /// <summary>
    /// Deletes poll from the repository.
    /// </summary>
    /// <param name="pollId">The ID of the poll to delete.</param>
    /// <returns>True if the poll was deleted successfully, false otherwise.</returns>
    bool DeletePoll(ulong pollId);

    /// <summary>
    /// Retrieves the poll from the repository.
    /// </summary>
    /// <param name="pollId">The ID of the poll to retrieve.</param>
    /// <param name="create">True to create a new poll if not found, false otherwise.</param>
    /// <returns>The retrieved poll or null if not found and create is false.</returns>
    PollHC? GetPoll(ulong pollId, bool create = false);

    /// <summary>
    /// Updates the poll in the repository.
    /// </summary>
    /// <param name="poll">The poll to be updated.</param>
    /// <returns>True if the poll was updated successfully, false otherwise.</returns>
    bool UpdatePoll(PollHC poll);
}

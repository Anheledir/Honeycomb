using BaseBotService.Data.Models;

namespace BaseBotService.Data.Interfaces;

/// <summary>
/// Interface for the MemberHC repository, providing CRUD operations for MemberHC entities.
/// </summary>
public interface IMemberHCRepository
{
    /// <summary>
    /// Retrieves a MemberHC entity by its user ID.
    /// </summary>
    /// <param name="userId">The user ID of the MemberHC entity to retrieve.</param>
    /// <returns>The MemberHC entity if found; otherwise, null.</returns>
    MemberHC GetUser(ulong userId);

    /// <summary>
    /// Adds a new MemberHC entity to the repository.
    /// </summary>
    /// <param name="user">The MemberHC entity to add.</param>
    void AddUser(MemberHC user);

    /// <summary>
    /// Updates an existing MemberHC entity in the repository.
    /// </summary>
    /// <param name="user">The MemberHC entity to update.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    bool UpdateUser(MemberHC user);

    /// <summary>
    /// Deletes a MemberHC entity by its user ID.
    /// </summary>
    /// <param name="userId">The user ID of the MemberHC entity to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    bool DeleteUser(ulong userId);
}
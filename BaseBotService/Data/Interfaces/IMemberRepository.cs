using BaseBotService.Data.Models;

namespace BaseBotService.Data.Interfaces;

/// <summary>
/// Interface for the MemberHC repository, providing CRUD operations for MemberHC entities.
/// </summary>
public interface IMemberRepository
{
    /// <summary>
    /// Asynchronously retrieves a MemberHC entity by its user ID.
    /// </summary>
    /// <param name="userId">The user ID of the MemberHC entity to retrieve.</param>
    /// <param name="create">If true, it will create a new user and return it if no user with <paramref name="userId"/> exists. Default is false.</param>
    /// <returns>A Task representing the asynchronous operation, with a MemberHC entity if found; otherwise, null. If <paramref name="create"/> is true, it will always return an entity.</returns>
    Task<MemberHC?> GetUserAsync(ulong userId, bool create = false);

    /// <summary>
    /// Asynchronously adds a new MemberHC entity to the repository.
    /// </summary>
    /// <param name="user">The MemberHC entity to add.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task AddUserAsync(MemberHC user);

    /// <summary>
    /// Asynchronously updates an existing MemberHC entity in the repository.
    /// </summary>
    /// <param name="user">The MemberHC entity to update.</param>
    /// <returns>A Task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> UpdateUserAsync(MemberHC user);

    /// <summary>
    /// Asynchronously deletes a MemberHC entity by its user ID.
    /// </summary>
    /// <param name="userId">The user ID of the MemberHC entity to delete.</param>
    /// <returns>A Task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
    Task<bool> DeleteUserAsync(ulong userId);
}

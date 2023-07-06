using BaseBotService.Core.Base;
using LiteDB;

namespace BaseBotService.Data.Models;

/// <summary>
/// Represents a single vote in the system.
/// </summary>
/// <remarks>
/// This class is used to store information about a single vote in the poll system.
/// </remarks>
public class PollVotesHC : ModelBase
{
    /// <summary>
    /// Gets or sets the ID of the poll.
    /// </summary>
    public ulong PollId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the voter.
    /// </summary>
    public ulong VoterId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the chosen option.
    /// </summary>
    public required ObjectId OptionId { get; set; }

    /// <summary>
    /// Gets or sets the datetime when the vote was cast.
    /// </summary>
    public DateTime VotedAt { get; set; }

    public static ILiteCollection<PollVotesHC> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<PollVotesHC> collection = GetServiceRegistration<PollVotesHC>(services);
        _ = collection.EnsureIndex(x => x.PollId, unique: false);
        _ = collection.EnsureIndex(x => x.OptionId, unique: false);
        _ = collection.EnsureIndex(x => new { x.PollId, x.OptionId }, unique: false);
        _ = collection.EnsureIndex(x => new { x.VoterId, x.PollId }, unique: false);
        return collection;
    }
}
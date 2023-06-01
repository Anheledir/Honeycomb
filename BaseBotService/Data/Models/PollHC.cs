using BaseBotService.Core.Base;

namespace BaseBotService.Data.Models;
/// <summary>
/// A class representing a poll in the system.
/// </summary>
public class PollHC : ModelBase
{
    /// <summary>
    /// The internal ID of the poll.
    /// </summary>
    /// <remarks>
    /// This equals the Discord message id which is used to display the poll.
    /// </remarks>
    public ulong PollId { get; set; }
    /// <summary>
    /// The start date of the poll.
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// The end date of the poll.
    /// </summary>
    public DateTime EndDate { get; set; }
    /// <summary>
    /// The ID of the guild where this poll was created.
    /// </summary>
    public ulong GuildId { get; set; }
    /// <summary>
    /// The ID of the creator of the poll.
    /// </summary>
    public ulong CreatorId { get; set; }
    /// <summary>
    /// The title of the poll.
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// The description of the poll.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// The IDs of the Discord roles that are allowed to vote in this poll.
    /// </summary>
    public List<ulong> AllowedVotingRoles { get; set; } = new List<ulong>();
    /// <summary>
    /// Specifies whether the results of the poll are public or not.
    /// </summary>
    public bool AreResultsPublic { get; set; }
    /// <summary>
    /// Specifies whether the poll is closed or not.
    /// </summary>
    public bool IsClosed { get; set; }
    /// <summary>
    /// Specifies whether the voters in the poll are hidden or not.
    /// </summary>
    public bool AreVotersHidden { get; set; }
    /// <summary>
    /// The different options for the poll.
    /// </summary>
    public List<PollOptions> Options { get; set; } = new List<PollOptions>();
    /// <summary>
    /// The votes received from each voter.
    /// </summary>
    public List<PollVotes> Votes { get; set; } = new List<PollVotes>();
}

public record PollOptions(short Id, string Emoji, string Text);
public record PollVotes(ulong VoterId, short OptionId, DateTime VotedAt);
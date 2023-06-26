using BaseBotService.Core.Base;
using LiteDB;

namespace BaseBotService.Data.Models;
/// <summary>
/// Represents available options for a poll.
/// </summary>
public class PollOptionsHC : ModelBase
{
    /// <summary>
    /// The ID of the poll this option belongs to.
    /// Usually equals the Discord message ID of the poll.
    /// </summary>
    public ulong PollId { get; set; }

    /// <summary>
    /// The description text of the poll option.
    /// </summary>
    public string Text { get; set; } = null!;

    /// <summary>
    /// The order of the option in the poll, relative to the other available options.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// The emoji that represents this option.
    /// </summary>
    public string Emoji { get; set; } = null!;

    public static ILiteCollection<PollOptionsHC> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<PollOptionsHC> collection = GetServiceRegistration<PollOptionsHC>(services);
        _ = collection.EnsureIndex(x => x.PollId, unique: false);
        _ = collection.EnsureIndex(x => new { x.Id, x.PollId }, unique: true);
        return collection;
    }
}

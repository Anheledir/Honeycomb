using BaseBotService.Core.Base;
using LiteDB;

namespace BaseBotService.Data.Models;

public class GuildMemberHC : ModelBase
{
    public ulong GuildId { get; set; }
    public ulong MemberId { get; set; }
    public DateTime LastActive { get; set; }
    public uint ActivityPoints { get; set; }
    public DateTime LastActivityPoint { get; set; }

    // Override the EnsureIndexes method to set up indexes on the collection
    protected override void EnsureIndexes<T>(ILiteCollection<T> collection)
    {
        var guildMemberCollection = collection as ILiteCollection<GuildMemberHC>;

        guildMemberCollection?.EnsureIndex(x => x.GuildId, unique: false);
        guildMemberCollection?.EnsureIndex(x => x.MemberId, unique: false);
        guildMemberCollection?.EnsureIndex(x => new { x.GuildId, x.MemberId }, unique: true);
    }
}

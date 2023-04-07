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
    public MemberHC Member { get; set; } = null!;
    public GuildHC Guild { get; set; } = null!;

    public static ILiteCollection<GuildMemberHC> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<GuildMemberHC> collection = GetServiceRegistration<GuildMemberHC>(services);
        _ = collection.EnsureIndex(x => x.GuildId, unique: false);
        _ = collection.EnsureIndex(x => x.MemberId, unique: false);
        _ = collection.EnsureIndex(x => new { x.GuildId, x.MemberId }, unique: true);
        return collection;
    }
}
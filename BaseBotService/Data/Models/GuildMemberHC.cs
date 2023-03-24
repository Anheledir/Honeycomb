using BaseBotService.Core.Base;
using LiteDB;

namespace BaseBotService.Data.Models;
public class GuildMemberHC : HCModelBase
{
    public ulong GuildId { get; set; }
    public ulong MemberId { get; set; }
    public DateTime LastActive { get; set; }
    public uint ActivityPoints { get; set; }
    public DateTime LastActivityPoint { get; set; }

    public static ILiteCollection<GuildMemberHC> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<GuildMemberHC> collection = GetServiceRegistration<GuildMemberHC>(services);
        _ = collection.EnsureIndex(x => x.GuildId);
        _ = collection.EnsureIndex(x => x.MemberId);
        _ = collection.EnsureIndex(x => new { x.GuildId, x.MemberId });
        return collection;
    }
}
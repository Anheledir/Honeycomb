using LiteDB;

namespace BaseBotService.Models;
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
        collection.EnsureIndex(x => x.GuildId);
        collection.EnsureIndex(x => x.MemberId);
        collection.EnsureIndex(x => new { x.GuildId, x.MemberId });
        return collection;
    }
}
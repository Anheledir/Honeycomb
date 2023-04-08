using BaseBotService.Core.Base;
using LiteDB;

namespace BaseBotService.Data.Models;
public class GuildMemberHC : ModelBase
{
    private GuildHC _guild = null!;
    private MemberHC _member = null!;

    public ulong GuildId { get; set; }
    public ulong MemberId { get; set; }
    public DateTime LastActive { get; set; }
    public uint ActivityPoints { get; set; }
    public DateTime LastActivityPoint { get; set; }
    public MemberHC Member
    {
        get => _member; set
        {
            _member = value;
            MemberId = value.MemberId;
        }
    }
    public GuildHC Guild
    {
        get => _guild;
        set
        {
            _guild = value;
            GuildId = value.GuildId;
        }
    }

    public static ILiteCollection<GuildMemberHC> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<GuildMemberHC> collection = GetServiceRegistration<GuildMemberHC>(services);
        _ = collection.EnsureIndex(x => x.GuildId, unique: false);
        _ = collection.EnsureIndex(x => x.MemberId, unique: false);
        _ = collection.EnsureIndex(x => new { x.GuildId, x.MemberId }, unique: true);
        return collection;
    }
}
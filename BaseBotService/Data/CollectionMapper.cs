using BaseBotService.Core.Base;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data;
internal static class CollectionMapper
{
    public static void RegisterCollections(ref BsonMapper mapper)
    {
        _ = mapper.Entity<GuildHC>()
            .DbRef(g => g.GuildMembers, nameof(GuildMemberHC));
        _ = mapper.Entity<GuildMemberHC>()
            .DbRef(u => u.Member, nameof(MemberHC))
            .DbRef(g => g.Guild, nameof(GuildHC));
        _ = mapper.Entity<MemberHC>()
            .DbRef(a => a.Achievements, nameof(AchievementBase));
        _ = mapper.Entity<AchievementBase>()
            .DbRef(u => u.Member, nameof(MemberHC))
            .DbRef(g => g.Guild, nameof(GuildHC));
    }
}

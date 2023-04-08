using BaseBotService.Core.Base;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data;
public class CollectionMapper
{
    public void RegisterCollections(ref BsonMapper mapper)
    {
        _ = mapper.Entity<GuildHC>()
            .DbRef(g => g.Members, nameof(GuildMemberHC));
        _ = mapper.Entity<GuildMemberHC>();
        _ = mapper.Entity<MemberHC>()
            .DbRef(a => a.Achievements, nameof(AchievementBase));
        _ = mapper.Entity<AchievementBase>();
    }
}

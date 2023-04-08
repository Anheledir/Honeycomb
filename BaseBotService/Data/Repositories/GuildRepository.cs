using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data.Repositories;

public class GuildRepository : IGuildRepository
{
    private readonly ILiteCollection<GuildHC> _guilds;

    public GuildRepository(ILiteCollection<GuildHC> guilds) => _guilds = guilds;

    public GuildHC? GetGuild(ulong guildId, bool create = false)
    {
        GuildHC? guild = _guilds
            .Include(m => m.Members)
            .FindOne(g => g.GuildId == guildId);
        if (guild == null && create)
        {
            _guilds.Insert(new GuildHC { GuildId = guildId });
            return GetGuild(guildId, false);
        }
        return guild;
    }

    public void AddGuild(GuildHC guild) => _guilds.Insert(guild);

    public bool UpdateGuild(GuildHC guild) => _guilds.Update(guild);

    public bool DeleteGuild(ulong guildId)
    {
        var guild = GetGuild(guildId);
        if (guild != null)
        {
            return _guilds.Delete(guild.Id);
        }
        return false;
    }
}

using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data;

public class GuildHCRepository : IGuildHCRepository
{
    private readonly ILiteCollection<GuildHC> _guilds;

    public GuildHCRepository(ILiteCollection<GuildHC> guilds)
    {
        _guilds = guilds;
    }

    public GuildHC? GetGuild(ulong guildId, bool touch = false)
    {
        GuildHC? guild = _guilds.FindOne(g => g.GuildId == guildId);
        if (guild == null && touch)
        {
            _guilds.Insert(new GuildHC { GuildId = guildId });
            guild = _guilds.FindOne(a => a.GuildId == guildId);
        }
        return guild;
    }

    public void AddGuild(GuildHC guild)
    {
        _guilds.Insert(guild);
    }

    public bool UpdateGuild(GuildHC guild)
    {
        return _guilds.Update(guild);
    }

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

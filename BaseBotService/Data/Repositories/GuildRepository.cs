using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseBotService.Data.Repositories;

public class GuildRepository : IGuildRepository
{
    private readonly HoneycombDbContext _context;

    public GuildRepository(HoneycombDbContext context)
    {
        _context = context;
    }

    public async Task<GuildHC?> GetGuildAsync(ulong guildId, bool create = false)
    {
        var guild = await _context.Guilds
                                  .Include(g => g.Members)
                                  .FirstOrDefaultAsync(g => g.GuildId == guildId);
        if (guild == null && create)
        {
            guild = new GuildHC { GuildId = guildId };
            _context.Guilds.Add(guild);
            await _context.SaveChangesAsync();
            return await GetGuildAsync(guildId, false);
        }
        return guild;
    }

    public async Task AddGuildAsync(GuildHC guild)
    {
        _context.Guilds.Add(guild);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateGuildAsync(GuildHC guild)
    {
        _context.Guilds.Update(guild);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteGuildAsync(ulong guildId)
    {
        var guild = await GetGuildAsync(guildId);
        if (guild != null)
        {
            _context.Guilds.Remove(guild);
            return await _context.SaveChangesAsync() > 0;
        }
        return false;
    }
}

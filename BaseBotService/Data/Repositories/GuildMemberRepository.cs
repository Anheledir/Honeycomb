using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseBotService.Data.Repositories;

public class GuildMemberRepository : IGuildMemberRepository
{
    private readonly HoneycombDbContext _context;

    public GuildMemberRepository(HoneycombDbContext context)
    {
        _context = context;
    }

    public async Task<GuildMemberHC?> GetUserAsync(ulong guildId, ulong userId)
    {
        return await _context.GuildMembers
                             .FirstOrDefaultAsync(gm => gm.GuildId == guildId && gm.MemberId == userId);
    }

    public async Task AddUserAsync(GuildMemberHC user)
    {
        _context.GuildMembers.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateUserAsync(GuildMemberHC user)
    {
        _context.GuildMembers.Update(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteUserAsync(ulong guildId, ulong userId)
    {
        var user = await GetUserAsync(guildId, userId);
        if (user != null)
        {
            _context.GuildMembers.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }
        return false;
    }

    public async Task<int> DeleteGuildAsync(ulong guildId)
    {
        var users = await _context.GuildMembers
                                  .Where(gm => gm.GuildId == guildId)
                                  .ToListAsync();

        if (users.Any())
        {
            _context.GuildMembers.RemoveRange(users);
            return await _context.SaveChangesAsync();
        }
        return 0;
    }
}

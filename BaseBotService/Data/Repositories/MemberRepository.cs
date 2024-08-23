using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseBotService.Data.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly HoneycombDbContext _context;

    public MemberRepository(HoneycombDbContext context)
    {
        _context = context;
    }

    public async Task<MemberHC?> GetUserAsync(ulong userId, bool create = false)
    {
        var user = await _context.Members
                                 .Include(m => m.Achievements)
                                 .FirstOrDefaultAsync(m => m.MemberId == userId);
        if (create && user == null)
        {
            user = new MemberHC { MemberId = userId };
            _context.Members.Add(user);
            await _context.SaveChangesAsync();
            user = await _context.Members
                                 .Include(m => m.Achievements)
                                 .FirstOrDefaultAsync(m => m.MemberId == userId);
        }
        return user;
    }

    public async Task AddUserAsync(MemberHC user)
    {
        _context.Members.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateUserAsync(MemberHC user)
    {
        _context.Members.Update(user);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteUserAsync(ulong userId)
    {
        var user = await GetUserAsync(userId);
        if (user != null)
        {
            _context.Members.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }
        return false;
    }
}
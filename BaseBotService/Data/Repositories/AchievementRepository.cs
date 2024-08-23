using BaseBotService.Core.Base;
using BaseBotService.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BaseBotService.Data.Repositories;

public class AchievementRepository<T> : IAchievementRepository<T> where T : AchievementBase
{
    private readonly HoneycombDbContext _context;
    public Guid Identifier => GetIdentifier();

    public AchievementRepository(HoneycombDbContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetByUserIdAsync(ulong userId)
    {
        return await _context.Achievements
            .Where(a => a.SourceIdentifier == Identifier && a.MemberId == userId)
            .OfType<T>()
            .ToListAsync();
    }

    public async Task<List<T>> GetByGuildIdAsync(ulong guildId)
    {
        return await _context.Achievements
            .Where(a => a.SourceIdentifier == Identifier && a.GuildId == guildId)
            .OfType<T>()
            .ToListAsync();
    }

    public async Task<List<T>> GetAsync(ulong userId, ulong guildId)
    {
        return await _context.Achievements
            .Where(a => a.SourceIdentifier == Identifier && a.MemberId == userId && a.GuildId == guildId)
            .OfType<T>()
            .ToListAsync();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Achievements
            .Where(a => a.SourceIdentifier == Identifier)
            .OfType<T>()
            .ToListAsync();
    }

    internal static Guid GetIdentifier()
    {
        const string propertyName = nameof(AchievementBase.Identifier);
        PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public)!;
        return new Guid((string)propertyInfo.GetValue(null)!);
    }

    public async Task<int> InsertAsync(T entity)
    {
        _context.Achievements.Add(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        _context.Achievements.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var achievement = await _context.Achievements.FindAsync(id);
        if (achievement != null)
        {
            _context.Achievements.Remove(achievement);
            return await _context.SaveChangesAsync() > 0;
        }
        return false;
    }
}

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
        // Uses the discriminator to filter the specific achievement type
        return await _context.Achievements
            .Where(a => a.SourceIdentifier == Identifier && a.MemberId == userId)
            .OfType<T>()
            .ToListAsync();
    }

    public async Task<List<T>> GetByGuildIdAsync(ulong guildId)
    {
        // Uses the discriminator to filter the specific achievement type
        return await _context.Achievements
            .Where(a => a.SourceIdentifier == Identifier && a.GuildId == guildId)
            .OfType<T>()
            .ToListAsync();
    }

    public async Task<List<T>> GetAsync(ulong userId, ulong guildId)
    {
        // Uses the discriminator to filter the specific achievement type
        return await _context.Achievements
            .Where(a => a.SourceIdentifier == Identifier && a.MemberId == userId && a.GuildId == guildId)
            .OfType<T>()
            .ToListAsync();
    }

    public async Task<List<T>> GetAllAsync()
    {
        // Retrieves all achievements of type T
        return await _context.Achievements
            .Where(a => a.SourceIdentifier == Identifier)
            .OfType<T>()
            .ToListAsync();
    }

    internal static Guid GetIdentifier()
    {
        const string propertyName = nameof(AchievementBase.Identifier);
        PropertyInfo propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);

        // Additional error handling to ensure the property exists
        if (propertyInfo == null)
        {
            throw new InvalidOperationException($"Identifier property not found on {typeof(T).Name}");
        }

        return new Guid((string)propertyInfo.GetValue(null)!);
    }

    public async Task<int> InsertAsync(T entity)
    {
        // Adds a new achievement entity to the database
        _context.Achievements.Add(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        // Updates an existing achievement entity in the database
        _context.Achievements.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        // Deletes an achievement entity based on its ID
        var achievement = await _context.Achievements.FindAsync(id);
        if (achievement != null)
        {
            _context.Achievements.Remove(achievement);
            return await _context.SaveChangesAsync() > 0;
        }
        return false;
    }
}

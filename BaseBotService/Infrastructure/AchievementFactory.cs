using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Infrastructure;

/// <summary>
/// A factory class responsible for creating and managing different achievement objects, such as the EasterEventAchievement.
/// </summary>
public static class AchievementFactory
{
    public static T CreateAchievement<T>(MemberHC user)
        where T : AchievementBase
        => CreateAchievement<T>(user.MemberId, null);

    public static T CreateAchievement<T>(GuildMemberHC guildUser)
        where T : AchievementBase
        => CreateAchievement<T>(guildUser.MemberId, guildUser.GuildId);

    /// <summary>
    /// Creates a new achievement of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the achievement. Must inherit <see cref="AchievementBase"/>.</typeparam>
    /// <returns>A new instance of the specified achievement type, inheriting <see cref="AchievementBase"/>.</returns>
    private static T CreateAchievement<T>(ulong userId, ulong? guildId) where T : AchievementBase
    {
        T? achievement = (T?)Program.ServiceProvider.GetService(typeof(T));
        if (achievement == null)
        {
            string error = $"Could not create achievement of type {typeof(T).Name}. Make sure the achievement is registered in the DI container.";
            Program.ServiceProvider.GetRequiredService<ILogger>().Error(error);
            throw new ArgumentException(error);
        }
        achievement.MemberId = userId;
        achievement.GuildId = guildId;
        achievement.CreatedAt = DateTime.UtcNow;
        return achievement;
    }
}

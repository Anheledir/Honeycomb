using BaseBotService.Core.Base;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Infrastructure;

/// <summary>
/// A factory class responsible for creating and managing different achievement objects, such as the EasterEventAchievement.
/// </summary>
public static class AchievementFactory
{
    /// <summary>
    /// Creates a new achievement of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the achievement. Must inherit <see cref="AchievementBase"/>.</typeparam>
    /// <param name="userId">The discord id of the user getting the achievement.</param>
    /// <param name="guildId">The discord id of the guild where the achievement was attributed to the user. Can be null for global achievements.</param>
    /// <returns>A new instance of the specified achievement type, inheriting <see cref="AchievementBase"/>.</returns>
    public static T CreateAchievement<T>(ulong userId, ulong? guildId = null) where T : AchievementBase
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
        achievement.CreatedAt = Instant.FromDateTimeUtc(DateTime.UtcNow);
        return achievement;
    }
}

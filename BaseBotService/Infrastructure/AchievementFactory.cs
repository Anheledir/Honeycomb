using BaseBotService.Core.Base;
using BaseBotService.Data.Models;

namespace BaseBotService.Infrastructure;

/// <summary>
/// A factory class responsible for creating and managing different achievement objects, such as the EasterEventAchievement.
/// </summary>
public class AchievementFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public AchievementFactory(IServiceProvider serviceProvider, ILogger logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public T CreateAchievement<T>(MemberHC user) where T : AchievementBase
    {
        return CreateAchievement<T>(user.MemberId, null);
    }

    public T CreateAchievement<T>(GuildMemberHC guildUser) where T : AchievementBase
    {
        return CreateAchievement<T>(guildUser.MemberId, guildUser.GuildId);
    }

    /// <summary>
    /// Creates a new achievement of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the achievement. Must inherit <see cref="AchievementBase"/>.</typeparam>
    /// <returns>A new instance of the specified achievement type, inheriting <see cref="AchievementBase"/>.</returns>
    private T CreateAchievement<T>(ulong userId, ulong? guildId) where T : AchievementBase
    {
        T? achievement = (T?)_serviceProvider.GetService(typeof(T));
        if (achievement == null)
        {
            string error = $"Could not create achievement of type {typeof(T).Name}. Make sure the achievement is registered in the DI container.";
            _logger.Error(error);
            throw new InvalidOperationException(error);
        }
        achievement.MemberId = userId;
        achievement.GuildId = guildId;
        achievement.CreatedAt = DateTime.UtcNow;
        return achievement;
    }
}

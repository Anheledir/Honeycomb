using Discord;

namespace BaseBotService.Interfaces;
public interface IActivityPointsService
{
    Task AddActivityTick(ulong guildId, ulong userId);
}
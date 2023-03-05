using Discord;

namespace BaseBotService.Interfaces;
public interface IActivityPointsService
{
    Task AddActivityTick(IGuildUser user);
}
namespace BaseBotService.Interfaces;
public interface IActivityPointsService
{
    Task AddActivityTick(ulong guildId, ulong userId);
    uint GetActivityPoints(ulong guildId, ulong userId);
    DateTime GetLastActive(ulong guildId, ulong userId);

}
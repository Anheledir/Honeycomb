﻿namespace BaseBotService.Commands.Interfaces;
public interface IEngagementService
{
    Task AddActivityTick(ulong guildId, ulong userId);
    uint GetActivityPoints(ulong guildId, ulong userId);
    DateTime GetLastActive(ulong guildId, ulong userId);
    int MaxPointsPerDay { get; }
}
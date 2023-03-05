using BaseBotService.Interfaces;
using BaseBotService.Models;
using Discord;
using LiteDB;
using Serilog;

namespace BaseBotService.Services;
public class ActivityPointsService : IActivityPointsService
{
    private readonly ILogger _logger;
    private readonly ILiteCollection<GuildMemberHC> _guildMembers;
    private readonly TimeSpan ActivityPointInterval = TimeSpan.FromSeconds(864);

    public ActivityPointsService(ILogger logger, ILiteCollection<GuildMemberHC> guildMembers)
    {
        _logger = logger;
        _guildMembers = guildMembers;
    }

    public Task AddActivityTick(IGuildUser user)
    {
        try
        {
            GuildMemberHC usr = _guildMembers.FindOne(a => a.GuildId == user.GuildId && a.MemberId == user.Id);
            if (usr == null)
            {
                // First time seeing this user in this guild
                _guildMembers.Insert(new GuildMemberHC { MemberId = user.Id, GuildId = user.GuildId, ActivityPoints = 1, LastActive = DateTime.UtcNow, LastActivityPoint = DateTime.UtcNow });
                _logger.Debug($"First activity tick for '{user.DisplayName}' ({user.Id}) in '{user.Guild.Name}' ({user.GuildId}).");
            }
            else
            {
                usr.LastActive = DateTime.UtcNow;

                if (usr.LastActivityPoint.Add(ActivityPointInterval) < DateTime.UtcNow)
                {
                    usr.ActivityPoints++;
                    usr.LastActivityPoint = DateTime.UtcNow;
                }

                _guildMembers.Update(usr);

                _logger.Debug($"Activity Tick for '{user.DisplayName}' ({user.Id}) in '{user.Guild.Name}' ({user.GuildId}), total points '{usr.ActivityPoints}'.");
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error adding activity tick for '{user.DisplayName}' ({user.Id}) in '{user.Guild.Name}' ({user.GuildId}).");
            throw;
        }
    }
}
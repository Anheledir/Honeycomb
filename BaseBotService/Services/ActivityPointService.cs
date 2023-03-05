using BaseBotService.Interfaces;
using BaseBotService.Models;
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

    public Task AddActivityTick(ulong guildId, ulong userId)
    {
        try
        {
            GuildMemberHC usr = _guildMembers.FindOne(a => a.GuildId == guildId && a.MemberId == userId);
            if (usr == null)
            {
                // First time seeing this user in this guild
                _logger.Information($"First time seeing user '{userId}' in '{guildId}'.");
                _guildMembers.Insert(new GuildMemberHC { MemberId = userId, GuildId = guildId, ActivityPoints = 1, LastActive = DateTime.UtcNow, LastActivityPoint = DateTime.UtcNow });
            }
            else
            {
                usr.LastActive = DateTime.UtcNow;

                if (usr.LastActivityPoint.Add(ActivityPointInterval) < DateTime.UtcNow)
                {
                    _logger.Information($"Adding activity tick to user '{userId}' in '{guildId}'.");
                    usr.ActivityPoints++;
                    usr.LastActivityPoint = DateTime.UtcNow;
                }

                _guildMembers.Update(usr);
            }
            return Task.CompletedTask;
        }
        catch (InvalidCastException ex)
        {
            // There was a casting error, probably because of some deprecated data
            _logger.Error(ex, $"Error happened in collection '{typeof(GuildMemberHC)}' for '{userId}' in '{guildId}'.");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error adding activity tick for '{userId}' in '{guildId}'.");
            throw;
        }
    }
}
using BaseBotService.Commands.Interfaces;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;

namespace BaseBotService.Commands;
public class EngagementService : IEngagementService
{
    private readonly ILogger _logger;
    private readonly TimeSpan _activityPointInterval = TimeSpan.FromSeconds(864);
    private readonly IGuildMemberRepository _guildMemberRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IGuildRepository _guildRepository;

    public int MaxPointsPerDay => 100;

    public EngagementService(ILogger logger, IGuildMemberRepository guildMemberRepository, IMemberRepository memberRepository, IGuildRepository guildRepository)
    {
        _logger = logger.ForContext<EngagementService>();
        _guildMemberRepository = guildMemberRepository;
        _memberRepository = memberRepository;
        _guildRepository = guildRepository;
    }

    public Task AddActivityTick(ulong guildId, ulong userId)
    {
        try
        {
            GuildMemberHC usr = _guildMemberRepository.GetUser(guildId, userId);
            if (usr == null)
            {
                // First time seeing this user in this guild
                _logger.Information($"First time seeing user '{userId}' in '{guildId}'.");
                var member = _memberRepository.GetUser(userId, true)!;
                var guild = _guildRepository.GetGuild(guildId, true)!;
                _guildMemberRepository.AddUser(new GuildMemberHC
                {
                    Member = member,
                    Guild = guild,
                    ActivityPoints = 1,
                    LastActive = DateTime.UtcNow,
                    LastActivityPoint = DateTime.UtcNow
                });
            }
            else
            {
                usr.LastActive = DateTime.UtcNow;

                if (usr.LastActivityPoint.Add(_activityPointInterval) < DateTime.UtcNow)
                {
                    _logger.Information($"Adding activity tick to user '{userId}' in '{guildId}'.");
                    usr.ActivityPoints++;
                    usr.LastActivityPoint = DateTime.UtcNow;
                }

                _ = _guildMemberRepository.UpdateUser(usr);
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

    public uint GetActivityPoints(ulong guildId, ulong userId)
    {
        try
        {
            GuildMemberHC usr = _guildMemberRepository.GetUser(guildId, userId);
            return usr?.ActivityPoints ?? 0;
        }
        catch (InvalidCastException ex)
        {
            // There was a casting error, probably because of some deprecated data
            _logger.Error(ex, $"Error happened in collection '{typeof(GuildMemberHC)}' for '{userId}' in '{guildId}'.");
            return 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error reading activity points for '{userId}' in '{guildId}'.");
            throw;
        }
    }

    public DateTime GetLastActive(ulong guildId, ulong userId)
    {
        try
        {
            GuildMemberHC usr = _guildMemberRepository.GetUser(guildId, userId);
            return usr?.LastActive ?? DateTime.MinValue;
        }
        catch (InvalidCastException ex)
        {
            // There was a casting error, probably because of some deprecated data
            _logger.Error(ex, $"Error happened in collection '{typeof(GuildMemberHC)}' for '{userId}' in '{guildId}'.");
            return DateTime.MinValue;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error reading last activity date/time for '{userId}' in '{guildId}'.");
            throw;
        }
    }
}
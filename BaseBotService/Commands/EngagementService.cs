using BaseBotService.Commands.Interfaces;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;

namespace BaseBotService.Commands;

public class EngagementService : IEngagementService
{
    private readonly ILogger _logger;
    private readonly TimeSpan _activityPointInterval = TimeSpan.FromSeconds(864); // Approx. 14 minutes and 24 seconds
    private readonly ILogger _logger1;
    private readonly IGuildMemberRepository _guildMemberRepository;
    private readonly IGuildRepository _guildRepository;
    private readonly IMemberRepository _memberRepository;

    public int MaxPointsPerDay => 100;

    public EngagementService(ILogger logger, IGuildMemberRepository guildMemberRepository, IGuildRepository guildRepository, IMemberRepository memberRepository)
    {
        _logger = logger.ForContext<EngagementService>();
        _logger1 = logger;
        _guildMemberRepository = guildMemberRepository;
        _guildRepository = guildRepository;
        _memberRepository = memberRepository;
    }

    public async Task AddActivityTickAsync(ulong guildId, ulong userId)
    {
        try
        {
            // Ensure the Guild exists
            var guild = await _guildRepository.GetGuildAsync(guildId);
            if (guild == null)
            {
                // Add the guild or handle the missing guild case
                guild = new GuildHC { GuildId = guildId };
                await _guildRepository.AddGuildAsync(guild);
            }

            // Ensure the Member exists
            var member = await _memberRepository.GetUserAsync(userId);
            if (member == null)
            {
                // Add the member or handle the missing member case
                member = new MemberHC { MemberId = userId };
                await _memberRepository.AddUserAsync(member);
            }

            GuildMemberHC? usr = await _guildMemberRepository.GetUserAsync(guildId, userId);
            if (usr == null)
            {
                // First time seeing this user in this guild
                _logger.Information($"First time seeing user '{userId}' in '{guildId}'.");
                usr = new GuildMemberHC
                {
                    MemberId = userId,
                    GuildId = guildId,
                    ActivityPoints = 1,
                    LastActive = DateTime.UtcNow,
                    LastActivityPoint = DateTime.UtcNow
                };
                await _guildMemberRepository.AddUserAsync(usr);
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

                await _guildMemberRepository.UpdateUserAsync(usr);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error adding activity tick for '{userId}' in '{guildId}'.");
            throw;
        }
    }

    public async Task<uint> GetActivityPointsAsync(ulong guildId, ulong userId)
    {
        try
        {
            GuildMemberHC? usr = await _guildMemberRepository.GetUserAsync(guildId, userId);
            return usr?.ActivityPoints ?? 0;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error reading activity points for '{userId}' in '{guildId}'.");
            throw;
        }
    }

    public async Task<DateTime> GetLastActiveAsync(ulong guildId, ulong userId)
    {
        try
        {
            GuildMemberHC? usr = await _guildMemberRepository.GetUserAsync(guildId, userId);
            return usr?.LastActive ?? DateTime.MinValue;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error reading last activity date/time for '{userId}' in '{guildId}'.");
            throw;
        }
    }
}

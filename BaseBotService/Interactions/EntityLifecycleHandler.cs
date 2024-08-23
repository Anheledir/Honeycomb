using BaseBotService.Commands.Interfaces;
using BaseBotService.Core;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;

namespace BaseBotService.Interactions;

/// <summary>
/// Handles lifecycle events related to guilds and users within the guilds.
/// </summary>
public class EntityLifecycleHandler :
    INotificationHandler<DiscordEventListener.BotJoinedGuildNotification>,
    INotificationHandler<DiscordEventListener.UserJoinedNotification>,
    INotificationHandler<DiscordEventListener.BotLeftGuildNotification>
{
    private readonly ILogger _logger;
    private readonly IMemberRepository _memberRepository;
    private readonly IGuildRepository _guildRepository;
    private readonly IGuildMemberRepository _guildMemberRepository;
    private readonly IEngagementService _engagementService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityLifecycleHandler"/> class.
    /// </summary>
    public EntityLifecycleHandler(
        ILogger logger,
        IMemberRepository memberRepository,
        IGuildRepository guildRepository,
        IGuildMemberRepository guildMemberRepository,
        IEngagementService engagementService)
    {
        _logger = logger.ForContext<EntityLifecycleHandler>();
        _memberRepository = memberRepository;
        _guildRepository = guildRepository;
        _guildMemberRepository = guildMemberRepository;
        _engagementService = engagementService;
    }

    /// <summary>
    /// Handles the event when a bot leaves a guild.
    /// </summary>
    public async Task Handle(DiscordEventListener.BotLeftGuildNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug($"{nameof(EntityLifecycleHandler)} received {nameof(DiscordEventListener.BotLeftGuildNotification)}");

        bool guildDeleted = await _guildRepository.DeleteGuildAsync(notification.Guild.Id);
        _logger.Information($"Bot left guild {notification.Guild.Id}, deleted GuildHC entity: {guildDeleted}.");

        int membersDeleted = await _guildMemberRepository.DeleteGuildAsync(notification.Guild.Id);
        _logger.Information($"Bot left guild {notification.Guild.Id}, deleted {membersDeleted} GuildMemberHC entities.");
    }

    /// <summary>
    /// Handles the event when a user joins a guild.
    /// </summary>
    public async Task Handle(DiscordEventListener.UserJoinedNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug($"{nameof(EntityLifecycleHandler)} received {nameof(DiscordEventListener.UserJoinedNotification)}");

        var member = await _memberRepository.GetUserAsync(notification.User.Id, true);
        await _engagementService.AddActivityTickAsync(notification.User.Guild.Id, notification.User.Id);
        _logger.Information($"User {notification.User.Id} joined {notification.User.Guild.Id}, created MemberHC (if necessary) and initialized first Activity-Tick.");
    }

    /// <summary>
    /// Handles the event when a bot joins a guild.
    /// </summary>
    public async Task Handle(DiscordEventListener.BotJoinedGuildNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug($"{nameof(EntityLifecycleHandler)} received {nameof(DiscordEventListener.BotJoinedGuildNotification)}");

        await _guildRepository.AddGuildAsync(new GuildHC { GuildId = notification.Guild.Id });
        _logger.Information($"Bot joined {notification.Guild.Id}, created GuildHC.");
    }
}

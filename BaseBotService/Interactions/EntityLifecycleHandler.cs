using BaseBotService.Commands.Interfaces;
using BaseBotService.Core.Messages;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;

namespace BaseBotService.Interactions;
public class EntityLifecycleHandler : INotificationHandler<JoinedGuildNotification>, INotificationHandler<UserJoinedNotification>, INotificationHandler<LeftGuildNotification>
{
    private readonly ILogger _logger;
    private readonly IMemberRepository _memberRepository;
    private readonly IGuildRepository _guildRepository;
    private readonly IGuildMemberRepository _guildMemberRepository;
    private readonly IEngagementService _engagementService;

    public EntityLifecycleHandler(ILogger logger, IMemberRepository memberRepository, IGuildRepository guildRepository, IGuildMemberRepository guildMemberRepository, IEngagementService engagementService)
    {
        _logger = logger.ForContext<EntityLifecycleHandler>();
        _memberRepository = memberRepository;
        _guildRepository = guildRepository;
        _guildMemberRepository = guildMemberRepository;
        _engagementService = engagementService;
    }

    public Task Handle(LeftGuildNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug($"{nameof(EntityLifecycleHandler)} received {nameof(LeftGuildNotification)}");

        bool success = _guildRepository.DeleteGuild(notification.Guild.Id);
        _logger.Information($"Bot left guild {notification.Guild.Id}, deleted GuildHC entity: {success}.");

        int amount = _guildMemberRepository.DeleteGuild(notification.Guild.Id);
        _logger.Information($"Bot left guild {notification.Guild.Id}, deleted {amount} GuildMemberHC entities.");

        return Task.CompletedTask;
    }

    public Task Handle(UserJoinedNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug($"{nameof(EntityLifecycleHandler)} received {nameof(UserJoinedNotification)}");

        _ = _memberRepository.GetUser(notification.User.Id, true)!;
        _engagementService.AddActivityTick(notification.User.Guild.Id, notification.User.Id);
        _logger.Information($"User {notification.User.Id} joined {notification.User.Guild.Id}, created MemberHC (if necessary) and initialized first Activity-Tick.");

        return Task.CompletedTask;
    }

    public Task Handle(JoinedGuildNotification notification, CancellationToken cancellationToken)
    {
        _logger.Debug($"{nameof(EntityLifecycleHandler)} received {nameof(JoinedGuildNotification)}");

        _guildRepository.AddGuild(new GuildHC { GuildId = notification.Guild.Id });
        _logger.Information($"Bot joined {notification.Guild.Id}, created GuildHC.");

        return Task.CompletedTask;
    }
}

using BaseBotService.Commands.Interfaces;
using BaseBotService.Core;
using BaseBotService.Core.Interfaces;
using BaseBotService.Core.Messages;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Infrastructure.Achievements;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Extensions;
using Discord.WebSocket;

namespace BaseBotService.Infrastructure.Services;

public class EasterEventService : IEasterEventService
{
    public const double ReactionProbability = 0.2;
    public const double AchievementRepeatedPointScaling = 0.1;

    private readonly List<string> _easterEmojis = new()
    {
        UnicodeEmojiHelper.hatchingChick,
        UnicodeEmojiHelper.babyChick,
        UnicodeEmojiHelper.frontFacingBabyChick,
        UnicodeEmojiHelper.rabbitFace,
        UnicodeEmojiHelper.rabbit,
        UnicodeEmojiHelper.egg,
        UnicodeEmojiHelper.tulip,
        UnicodeEmojiHelper.cherryBlossom,
        UnicodeEmojiHelper.blossom,
        UnicodeEmojiHelper.sunflower,
        UnicodeEmojiHelper.hibiscus,
        UnicodeEmojiHelper.chocolateBar,
        UnicodeEmojiHelper.candy,
        UnicodeEmojiHelper.lollipop
    };

    private readonly ILogger _logger;
    private readonly ILogger _logger1;
    private readonly IDateTimeProvider _dateTime;
    private readonly DiscordSocketClient _client;
    private readonly IMemberRepository _memberHCRepository;
    private readonly ITranslationService _translationService;
    private readonly IAchievementRepository<EasterEventAchievement> _easterEventAchievements;
    private readonly IGuildMemberRepository _guildMemberRepository;
    private readonly IEngagementService _engagementService;
    private readonly IMediator _mediator;
    private readonly AchievementFactory _achievementFactory;
    private readonly Random _random;

    public EasterEventService(ILogger logger, IDateTimeProvider dateTime, DiscordSocketClient client, IMemberRepository memberHCRepository, ITranslationService translationService, IAchievementRepository<EasterEventAchievement> easterEventAchievements, IGuildMemberRepository guildMemberRepository, IEngagementService engagementService, IMediator mediator, AchievementFactory achievementFactory)
    {
        _logger = logger.ForContext<EasterEventService>();
        _logger1 = logger;
        _dateTime = dateTime;
        _client = client;
        _memberHCRepository = memberHCRepository;
        _translationService = translationService;
        _easterEventAchievements = easterEventAchievements;
        _guildMemberRepository = guildMemberRepository;
        _engagementService = engagementService;
        _mediator = mediator;
        _achievementFactory = achievementFactory;
        _random = new Random();
        logger.Debug($"Initialized {nameof(EasterEventService)}");
    }

    internal bool IsEasterPeriod(bool doIncludeMonday = false) => doIncludeMonday
        ? _dateTime.IsEasterSunday(_dateTime.GetCurrentUtcDate()) || _dateTime.IsEasterMonday(_dateTime.GetCurrentUtcDate())
        : _dateTime.IsEasterSunday(_dateTime.GetCurrentUtcDate());

    public async Task HandleMessageReceivedAsync(MessageReceivedNotification arg)
    {
        if (arg.Message.Author.IsBot || arg.Message.Author.IsWebhook || !IsEasterPeriod()) return;

        await _mediator.Publish(new DiscordEventListener.UpdateActivityNotification(
            _translationService.GetAttrString(EasterEventAchievement.TranslationKey, "activity"),
            UserStatus.Online,
            new Emoji(_translationService.GetAttrString(EasterEventAchievement.TranslationKey, "emoji"))
        ));

        var rnd = _random.NextDouble();
        _logger.Debug($"Rolling the dice [{ReactionProbability:P1}]: {rnd} ");
        if (rnd < ReactionProbability)
        {
            IEmote easterEmoji = new Emoji(_easterEmojis.GetRandomItem());
            await arg.Message.AddReactionAsync(easterEmoji);
        }
    }

    public async Task HandleReactionAddedAsync(DiscordEventListener.ReactionAddedNotification arg)
    {
        _logger.Debug("HandleReactionAddedAsync: Check 1");
        var channel = await arg.Channel.GetOrDownloadAsync();
        if (channel is IDMChannel) return;

        _logger.Debug("HandleReactionAddedAsync: Check 2");
        if (arg.Reaction.UserId == _client.CurrentUser.Id) return;

        _logger.Debug("HandleReactionAddedAsync: Check 3");
        if (!_easterEmojis.Contains(arg.Reaction.Emote.Name)) return;

        _logger.Debug("HandleReactionAddedAsync: Check 4");
        IUser user = arg.Reaction.User.IsSpecified ? arg.Reaction.User.Value : await _client.GetUserAsync(arg.Reaction.UserId);
        if (user.IsBot || user.IsWebhook) return;

        _logger.Debug("HandleReactionAddedAsync: Check 5");
        var message = await arg.Message.GetOrDownloadAsync();
        if (message.Author.IsBot) return;

        _logger.Debug("HandleReactionAddedAsync: Check 6");
        bool isEventReaction = await message.GetReactionUsersAsync(arg.Reaction.Emote, 50)
            .FlattenAsync()
            .ContinueWith(r => r.Result.Any(u => u.Id == _client.CurrentUser.Id));
        if (!isEventReaction) return;

        _logger.Debug("HandleReactionAddedAsync: Check 7");
        if (channel is not SocketGuildChannel guildChannel) return;
        SocketGuild guild = guildChannel.Guild;
        MemberHC? memberHC = await _memberHCRepository.GetUserAsync(user.Id, true);
        var userAchievements = await _easterEventAchievements.GetByUserIdAsync(memberHC.MemberId);
        await _engagementService.AddActivityTickAsync(guild.Id, memberHC.MemberId);
        var gUsr = await _guildMemberRepository.GetUserAsync(guild.Id, memberHC.MemberId);

        _logger.Debug("HandleReactionAddedAsync: Check 8");
        int currentYear = _dateTime.GetCurrentUtcDate().Year;
        EasterEventAchievement? existingAchievement =
            userAchievements.Find(a => a.GuildId == guild.Id && a.CreatedAt.Year == currentYear);

        await message.RemoveReactionAsync(arg.Reaction.Emote, _client.CurrentUser);

        if (existingAchievement != null)
        {
            uint scaledPoints = (uint)(existingAchievement.Points * AchievementRepeatedPointScaling);
            _logger.Debug($"User {user.Id} already has the {nameof(EasterEventAchievement)} in {guild.Id}. Adding {AchievementRepeatedPointScaling:P1} of {existingAchievement.Points} points = {scaledPoints} points.");
            gUsr.ActivityPoints += scaledPoints;
            await _guildMemberRepository.UpdateUserAsync(gUsr);
            return;
        }

        _logger.Information($"The user {user.Id} on {guild.Id} got the {nameof(EasterEventAchievement)}.");
        EasterEventAchievement achievement = _achievementFactory.CreateAchievement<EasterEventAchievement>(gUsr!);
        await _easterEventAchievements.InsertAsync(achievement);
        memberHC.Achievements.Add(achievement);
        gUsr.ActivityPoints += (uint)achievement.Points;
        bool update = await _memberHCRepository.UpdateUserAsync(memberHC) && await _guildMemberRepository.UpdateUserAsync(gUsr);
        if (!update)
        {
            _logger.Warning($"Could not update user {memberHC.MemberId} after adding {nameof(EasterEventAchievement)}");
        }
        else
        {
            _logger.Debug($"Updated user {memberHC.MemberId} after adding {nameof(EasterEventAchievement)}");
        }

        var arguments = TranslationHelper.Arguments("username", user.Mention);
        await message.Channel.SendMessageAsync(_translationService.GetAttrString(EasterEventAchievement.TranslationKey, "notification", arguments));
    }
}

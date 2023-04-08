using BaseBotService.Commands.Interfaces;
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
    /// <summary>
    /// The probability of reacting to a message (0.05 = 5%)
    /// </summary>
    public const double ReactionProbability = 0.05;

    /// <summary>
    /// If the user has already received the achievement, the points will be scaled down by this factor (0.1 = 10%)
    /// </summary>
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
    private readonly IDateTimeProvider _dateTime;
    private readonly DiscordSocketClient _client;
    private readonly IMemberRepository _memberHCRepository;
    private readonly ITranslationService _translationService;
    private readonly IAchievementRepository<EasterEventAchievement> _easterEventAchievements;
    private readonly IGuildMemberRepository _guildMemberRepository;
    private readonly IEngagementService _engagementService;
    private readonly IMediator _mediator;
    private readonly Random _random;

    public EasterEventService(ILogger logger, IDateTimeProvider dateTime, DiscordSocketClient client, IMemberRepository memberHCRepository, ITranslationService translationService, IAchievementRepository<EasterEventAchievement> easterEventAchievements, IGuildMemberRepository guildMemberRepository, IEngagementService engagementService, IMediator mediator)
    {
        _logger = logger.ForContext<EasterEventService>();
        _dateTime = dateTime;
        _client = client;
        _memberHCRepository = memberHCRepository;
        _translationService = translationService;
        _easterEventAchievements = easterEventAchievements;
        _guildMemberRepository = guildMemberRepository;
        _engagementService = engagementService;
        _mediator = mediator;
        _random = new Random();
        logger.Debug($"Initialized {nameof(EasterEventService)}");
    }

    internal bool IsEasterPeriod(bool doIncludeMonday = false) => doIncludeMonday
        ? _dateTime.IsEasterSunday(_dateTime.GetCurrentUtcDate()) || _dateTime.IsEasterMonday(_dateTime.GetCurrentUtcDate())
        : _dateTime.IsEasterSunday(_dateTime.GetCurrentUtcDate());

    public async Task HandleMessageReceivedAsync(MessageReceivedNotification arg)
    {
        if (arg.Message.Author.IsBot || arg.Message.Author.IsWebhook || !IsEasterPeriod()) return;

        await _mediator.Publish(new UpdateActivityNotification
        {
            ActivityType = ActivityType.CustomStatus,
            Status = UserStatus.Online,
            Emote = new Emoji(_translationService.GetAttrString(EasterEventAchievement.TranslationKey, "emoji")),
            Description = _translationService.GetAttrString(EasterEventAchievement.TranslationKey, "activity")
        });

        var rnd = _random.NextDouble();
        _logger.Debug($"Rolling the dice [{ReactionProbability:P1}]: {rnd} ");
        if (rnd < ReactionProbability)
        {
            IEmote easterEmoji = new Emoji(_easterEmojis.GetRandomItem());
            await arg.Message.AddReactionAsync(easterEmoji);
        }
    }

    public async Task HandleReactionAddedAsync(ReactionAddedNotification arg)
    {
        _logger.Debug("HandleReactionAddedAsync: Check 1");
        // Messages within a DM are not eligible
        var channel = await arg.Channel.GetOrDownloadAsync();
        if (channel is IDMChannel) return;

        _logger.Debug("HandleReactionAddedAsync: Check 2");
        // Reactions made by the bot are not eligible
        if (arg.Reaction.UserId == _client.CurrentUser.Id) return;

        _logger.Debug("HandleReactionAddedAsync: Check 3");
        // Check if the reaction is in the list of eligible ones
        if (!_easterEmojis.Contains(arg.Reaction.Emote.Name)) return;

        _logger.Debug("HandleReactionAddedAsync: Check 4");
        // Reaction must be made by a user
        IUser user = arg.Reaction.User.IsSpecified ? arg.Reaction.User.Value : await _client.GetUserAsync(arg.Reaction.UserId);
        if (user.IsBot || user.IsWebhook) return;

        _logger.Debug("HandleReactionAddedAsync: Check 5");
        // Message must be made by a user, too
        var message = await arg.Message.GetOrDownloadAsync();
        if (message.Author.IsBot) return;

        _logger.Debug("HandleReactionAddedAsync: Check 6");
        // Make sure the initial reaction was made by the bot
        bool isEventReaction = await message.GetReactionUsersAsync(arg.Reaction.Emote, 50)
            .FlattenAsync()
            .ContinueWith(r => r.Result.Any(u => u.Id == _client.CurrentUser.Id));
        if (!isEventReaction) return;

        _logger.Debug("HandleReactionAddedAsync: Check 7");
        // Only messages within a guild are eligible
        if (channel is not SocketGuildChannel guildChannel) return;
        SocketGuild guild = guildChannel.Guild;
        MemberHC memberHC = _memberHCRepository.GetUser(user.Id, true)!;
        List<EasterEventAchievement> userAchievements = memberHC.GetAchievements(_easterEventAchievements);
        _ = _engagementService.AddActivityTick(guild.Id, memberHC.MemberId);
        var gUsr = _guildMemberRepository.GetUser(guild.Id, memberHC.MemberId);

        _logger.Debug("HandleReactionAddedAsync: Check 8");
        // A user can only get this achievement once per guild and year
        int currentYear = _dateTime.GetCurrentUtcDate().Year;
        EasterEventAchievement? existingAchievement =
            userAchievements.Find(a => a.GuildId == guild.Id && a.CreatedAt.Year == currentYear);

        // Hooray, we made it this far! Remove the bots reaction again as it can only be redeemed once
        await message.RemoveReactionAsync(arg.Reaction.Emote, _client.CurrentUser);

        if (existingAchievement != null)
        {
            uint scaledPoints = (uint)(existingAchievement.Points * AchievementRepeatedPointScaling);
            _logger.Debug($"User {user.Id} already has the {nameof(EasterEventAchievement)} in {guild.Id}. Adding {AchievementRepeatedPointScaling:P1} of {existingAchievement.Points} points = {scaledPoints} points.");
            gUsr.ActivityPoints += scaledPoints;
            _guildMemberRepository.UpdateUser(gUsr);
            return;
        }

        _logger.Information($"The user {user.Id} on {guild.Id} got the {nameof(EasterEventAchievement)}.");
        EasterEventAchievement achievement = AchievementFactory.CreateAchievement<EasterEventAchievement>(gUsr);
        achievement.Id = _easterEventAchievements.Insert(achievement);
        memberHC.Achievements.Add(achievement);
        gUsr.ActivityPoints += (uint)achievement.Points;
        bool update = _memberHCRepository.UpdateUser(memberHC) && _guildMemberRepository.UpdateUser(gUsr);
        if (!update)
        {
            _logger.Warning($"Could not update user {memberHC.MemberId} after adding {nameof(EasterEventAchievement)}");
        }
        else
        {
            _logger.Debug($"Updated user {memberHC.MemberId} after adding {nameof(EasterEventAchievement)}");
        }

        // Send achievement notification message at last
        var arguments = TranslationHelper.Arguments("username", user.Mention);
        await message.Channel.SendMessageAsync(_translationService.GetAttrString(EasterEventAchievement.TranslationKey, "notification", arguments));
    }
}

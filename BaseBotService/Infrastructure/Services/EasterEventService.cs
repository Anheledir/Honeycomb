using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Core.Messages;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Infrastructure.Achievements;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Extensions;
using Discord.WebSocket;

namespace BaseBotService.Infrastructure.Services;
public class EasterEventService
{
    /// <summary>
    /// The probability of reacting to a message (0.1 = 10%)
    /// </summary>
    public const double ReactionProbability = 0.05;

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
    private readonly IMemberHCRepository _memberHCRepository;
    private readonly ITranslationService _translationService;

    public EasterEventService(ILogger logger, IDateTimeProvider dateTime, DiscordSocketClient client, IMemberHCRepository memberHCRepository, ITranslationService translationService)
    {
        _logger = logger.ForContext<EasterEventService>();
        _dateTime = dateTime;
        _client = client;
        _memberHCRepository = memberHCRepository;
        _translationService = translationService;
        logger.Debug($"Initialized {nameof(EasterEventService)}");
    }

    internal bool IsEasterPeriod(bool doIncludeMonday = false) => doIncludeMonday
        ? _dateTime.IsEasterSunday(_dateTime.GetCurrentUtcDate()) || _dateTime.IsEasterMonday(_dateTime.GetCurrentUtcDate())
        : _dateTime.IsEasterSunday(_dateTime.GetCurrentUtcDate());

    public async Task HandleMessageReceivedAsync(MessageReceivedNotification arg)
    {
        if (arg.Message is not SocketMessage message) return;

        if (message.Author.IsBot || message.Author.IsWebhook || !IsEasterPeriod()) return;

        var random = new Random();
        if (random.NextDouble() < ReactionProbability)
        {
            IEmote easterEmoji = new Emoji(_easterEmojis.GetRandomItem());
            await message.AddReactionAsync(easterEmoji);
        }
    }

    internal async Task HandleReactionAddedAsync(ReactionAddedNotification arg)
    {
        // Messages within a DM are not eligible
        var channel = await arg.Channel.GetOrDownloadAsync();
        if (channel is IDMChannel) return;

        // Reactions made by the bot are not eligible
        if (arg.Reaction.UserId == _client.CurrentUser.Id) return;

        // Check if the reaction is in the list of eligible ones
        if (!_easterEmojis.Contains(arg.Reaction.Emote.Name)) return;

        // Reaction must be made by a user
        IUser user = arg.Reaction.User.IsSpecified ? arg.Reaction.User.Value : await _client.GetUserAsync(arg.Reaction.UserId);
        if (user.IsBot || user.IsWebhook) return;

        // Message must be made by a user, too
        var message = await arg.Message.GetOrDownloadAsync();
        if (message.Author.IsBot) return;

        // Make sure the initial reaction was made by the bot
        bool isEventReaction = await message.GetReactionUsersAsync(arg.Reaction.Emote, 2)
            .FlattenAsync()
            .ContinueWith(r => r.Result.Any(u => u.Id == _client.CurrentUser.Id));
        if (!isEventReaction) return;

        // Only messages within a guild are eligible
        if (channel is not SocketGuildChannel guildChannel) return;
        SocketGuild guild = guildChannel.Guild;
        MemberHC memberHC = _memberHCRepository.GetUser(user.Id, true)!;
        List<HCAchievementBase> userAchievements = memberHC.Achievements.Where(a => a.SourceIdentifier == new Guid(EasterEventAchievement.Identifier)).ToList();

        // A user can only get this achievement once per guild and year
        int currentYear = _dateTime.GetCurrentUtcDate().Year;
        if (userAchievements.Any(a => a.GuildId == guild.Id && a.CreatedAt.InUtc().Year == currentYear)) return;

        // Hooray, we made it this far! Remove the bots reaction again as it can only be redeemed once
        await message.RemoveReactionAsync(arg.Reaction.Emote, _client.CurrentUser);

        EasterEventAchievement achievement = AchievementFactory.CreateAchievement<EasterEventAchievement>(user.Id, guild.Id);
        _logger.Information($"The user {user.Id} on {guild.Id} got the {nameof(EasterEventAchievement)}");
        memberHC.Achievements.Add(achievement);
        bool update = _memberHCRepository.UpdateUser(memberHC);
        if (!update)
        {
            _logger.Warning("Could not update user {userid} after adding {@achievement}", memberHC.MemberId, achievement);
        }
        else
        {
            _logger.Debug("Updated user {userid} after adding {@achievement}", memberHC.MemberId, achievement);
        }

        // Send achievement notification message at last
        await user.SendMessageAsync(_translationService.GetAttrString(EasterEventAchievement.TranslationKey, "notification"));
    }
}

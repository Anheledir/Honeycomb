using BaseBotService.Commands.Interfaces;
using BaseBotService.Core.Interfaces;
using BaseBotService.Core.Messages;
using BaseBotService.Data.Interfaces;
using BaseBotService.Infrastructure.Achievements;
using BaseBotService.Infrastructure.Services;
using BaseBotService.Utilities.Extensions;
using Discord;
using Discord.WebSocket;
using MediatR;
using Serilog;

namespace BaseBotService.Tests.Infrastructure.Services;

public class EasterEventServiceTests
{
    private EasterEventService _easterEventService;
    private ILogger _logger;
    private IDateTimeProvider _dateTime;
    private DiscordSocketClient _client;
    private IMemberRepository _memberHCRepository;
    private ITranslationService _translationService;
    private IAchievementRepository<EasterEventAchievement> _achievementRepository;
    private IGuildMemberRepository _guildMemberHCRepository;
    private IEngagementService _engagementService;
    private IMediator _mediator;
    private Faker _faker;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
        _dateTime = Substitute.For<IDateTimeProvider>();
        _client = Substitute.For<DiscordSocketClient>();
        _memberHCRepository = Substitute.For<IMemberRepository>();
        _translationService = Substitute.For<ITranslationService>();
        _achievementRepository = Substitute.For<IAchievementRepository<EasterEventAchievement>>();
        _guildMemberHCRepository = Substitute.For<IGuildMemberRepository>();
        _engagementService = Substitute.For<IEngagementService>();
        _mediator = Substitute.For<IMediator>();

        _easterEventService = new EasterEventService(_logger, _dateTime, _client, _memberHCRepository, _translationService, _achievementRepository, _guildMemberHCRepository, _engagementService, _mediator);

        _faker = new Faker();
    }

    [Test]
    public void IsEasterPeriod_WhenEasterSunday_ReturnsTrue()
    {
        // Arrange
        var easterSunday = _faker.Date.Future().ToLocalDate();
        _dateTime.GetCurrentUtcDate().Returns(easterSunday);
        _dateTime.IsEasterSunday(Arg.Is<LocalDate>(d => d == easterSunday)).Returns(true);

        // Act
        bool isEasterPeriod = _easterEventService.IsEasterPeriod(doIncludeMonday: false);

        // Assert
        isEasterPeriod.ShouldBeTrue();
    }

    [Test]
    public void IsEasterPeriod_WhenNotEaster_ReturnsFalse()
    {
        // Arrange
        var notEaster = _faker.Date.Future().ToLocalDate();
        _dateTime.GetCurrentUtcDate().Returns(notEaster);
        _dateTime.IsEasterSunday(Arg.Is<LocalDate>(d => d == notEaster)).Returns(false);

        // Act
        bool isEasterPeriod = _easterEventService.IsEasterPeriod(doIncludeMonday: false);

        // Assert
        isEasterPeriod.ShouldBeFalse();
    }

    [Test]
    public void IsEasterPeriod_WhenEasterMondayAndIncludingMonday_ReturnsTrue()
    {
        // Arrange
        var easterMonday = _faker.Date.Future().ToLocalDate();
        _dateTime.GetCurrentUtcDate().Returns(easterMonday);
        _dateTime.IsEasterMonday(Arg.Is<LocalDate>(d => d == easterMonday)).Returns(true);

        // Act
        bool isEasterPeriod = _easterEventService.IsEasterPeriod(doIncludeMonday: true);

        // Assert
        isEasterPeriod.ShouldBeTrue();
    }

    [Test]
    public void IsEasterPeriod_WhenEasterMondayAndExcludingMonday_ReturnsFalse()
    {
        // Arrange
        var easterMonday = _faker.Date.Future().ToLocalDate();
        _dateTime.GetCurrentUtcDate().Returns(easterMonday);
        _dateTime.IsEasterMonday(Arg.Is<LocalDate>(d => d == easterMonday)).Returns(true);

        // Act
        bool isEasterPeriod = _easterEventService.IsEasterPeriod(doIncludeMonday: false);

        // Assert
        isEasterPeriod.ShouldBeFalse();
    }

    [Test]
    public async Task HandleMessageReceivedAsync_MessageAuthorIsBot_DoesNotAddReaction()
    {
        // Arrange
        var message = Substitute.For<IMessage>();
        message.Author.IsBot.Returns(true);

        var notification = new MessageReceivedNotification(message);

        // Act
        await _easterEventService.HandleMessageReceivedAsync(notification);

        // Assert
        await message.DidNotReceive().AddReactionAsync(Arg.Any<IEmote>());
    }

    [Test]
    public async Task HandleMessageReceivedAsync_NotEasterPeriod_DoesNotAddReaction()
    {
        // Arrange
        _dateTime.IsEasterSunday(Arg.Any<LocalDate>()).Returns(false);

        var message = Substitute.For<IMessage>();
        message.Author.IsBot.Returns(false);
        message.Author.IsWebhook.Returns(false);

        var notification = new MessageReceivedNotification(message);

        // Act
        await _easterEventService.HandleMessageReceivedAsync(notification);

        // Assert
        await message.DidNotReceive().AddReactionAsync(Arg.Any<IEmote>());
    }
}
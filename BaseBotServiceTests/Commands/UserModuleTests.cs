using BaseBotService.Commands;
using BaseBotService.Commands.Interfaces;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using Discord;
using Serilog;

namespace BaseBotService.Tests.Commands;

[TestFixture]
public class UserModuleTests
{
    private ITranslationService _translationService;
    private IEngagementService _engagementService;
    private IMemberRepository _memberRepository;
    private ILogger _logger;
    private UserModule _userModule;
    private readonly Faker _faker = new();

    [SetUp]
    public void SetUp()
    {
        _translationService = Substitute.For<ITranslationService>();
        _engagementService = Substitute.For<IEngagementService>();
        _memberRepository = Substitute.For<IMemberRepository>();
        _logger = Substitute.For<ILogger>();

        _userModule = new UserModule(_logger, _translationService, _engagementService, _memberRepository);
    }

    [Test]
    public void GetActivityScore_ShouldReturnZero_WhenUserOrBotJoinedAtIsMissing()
    {
        // Arrange
        var user = Substitute.For<IGuildUser>();
        user.JoinedAt.Returns((DateTimeOffset?)null);
        var bot = Substitute.For<IGuildUser>();
        bot.JoinedAt.Returns((DateTimeOffset?)null);

        // Act
        double result = _userModule.GetActivityScoreAsync(user, bot).Result;

        // Assert
        result.ShouldBe(0);
    }

    [Test]
    public void GetActivityScore_ShouldReturnZero_WhenUserJoinedAtIsLaterThanBotJoinedAt()
    {
        // Arrange
        var userJoinedDate = DateTimeOffset.UtcNow.AddDays(-1);
        var botJoinedDate = DateTimeOffset.UtcNow.AddDays(-5);

        var user = Substitute.For<IGuildUser>();
        user.JoinedAt.Returns(userJoinedDate);

        var bot = Substitute.For<IGuildUser>();
        bot.JoinedAt.Returns(botJoinedDate);

        // Act
        double result = _userModule.GetActivityScoreAsync(user, bot).Result;

        // Assert
        result.ShouldBe(0);
    }

    [TestCase((uint)85, 100)]
    [TestCase((uint)125, 100)]
    [TestCase((uint)42, 50)]
    [TestCase((uint)9, 10)]
    public void GetActivityScore_ShouldCalculateCorrectScore_WhenUserAndBotHaveValidJoinedAtDates(uint points, double expectedScore)
    {
        // Arrange
        var userJoinedDate = DateTimeOffset.UtcNow.AddDays(-5);
        var botJoinedDate = DateTimeOffset.UtcNow.AddDays(-10);
        ulong guildId = _faker.Random.ULong();
        ulong userId = _faker.Random.ULong();
        ulong botId = _faker.Random.ULong();

        var user = Substitute.For<IGuildUser>();
        user.JoinedAt.Returns(userJoinedDate);
        user.GuildId.Returns(guildId);
        user.Id.Returns(userId);

        var bot = Substitute.For<IGuildUser>();
        bot.JoinedAt.Returns(botJoinedDate);
        bot.GuildId.Returns(guildId);
        bot.Id.Returns(botId);

        _engagementService.MaxPointsPerDay.Returns(100);
        _engagementService.GetActivityPointsAsync(guildId, userId).Result.Returns(points);

        // Act
        double result = _userModule.GetActivityScoreAsync(user, bot).Result;

        // Assert
        result.ShouldBe(expectedScore, 1.0);
    }

    [Test]
    public void GetActivityScore_ShouldReturnMaxScore_WhenPointsExceedMaxPointsPerDay()
    {
        // Arrange
        var userJoinedDate = DateTimeOffset.UtcNow.AddDays(-5);
        var botJoinedDate = DateTimeOffset.UtcNow.AddDays(-10);
        ulong guildId = _faker.Random.ULong();
        ulong userId = _faker.Random.ULong();

        var user = Substitute.For<IGuildUser>();
        user.JoinedAt.Returns(userJoinedDate);
        user.GuildId.Returns(guildId);
        user.Id.Returns(userId);

        var bot = Substitute.For<IGuildUser>();
        bot.JoinedAt.Returns(botJoinedDate);
        bot.GuildId.Returns(guildId);

        _engagementService.MaxPointsPerDay.Returns(100);
        _engagementService.GetActivityPointsAsync(guildId, userId).Result.Returns((uint)150); // Exceeding MaxPointsPerDay

        // Act
        double result = _userModule.GetActivityScoreAsync(user, bot).Result;

        // Assert
        result.ShouldBe(100);
    }

    [Test]
    public void GetActivityScore_ShouldHandleSameJoinDatesCorrectly()
    {
        // Arrange
        var joinDate = DateTimeOffset.UtcNow.AddDays(-5);
        ulong guildId = _faker.Random.ULong();
        ulong userId = _faker.Random.ULong();

        var user = Substitute.For<IGuildUser>();
        user.JoinedAt.Returns(joinDate);
        user.GuildId.Returns(guildId);
        user.Id.Returns(userId);

        var bot = Substitute.For<IGuildUser>();
        bot.JoinedAt.Returns(joinDate);
        bot.GuildId.Returns(guildId);

        _engagementService.MaxPointsPerDay.Returns(100);
        _engagementService.GetActivityPointsAsync(guildId, userId).Result.Returns((uint)50);

        // Act
        double result = _userModule.GetActivityScoreAsync(user, bot).Result;

        // Assert
        result.ShouldBe(50, 1.0);
    }

    [Test]
    public void GetActivityScore_ShouldReturnZero_WhenUserJoinedAtExactlyOneDayBeforeBot()
    {
        // Arrange
        var userJoinedDate = DateTimeOffset.UtcNow.AddDays(-5);
        var botJoinedDate = DateTimeOffset.UtcNow.AddDays(-4);
        ulong guildId = _faker.Random.ULong();
        ulong userId = _faker.Random.ULong();

        var user = Substitute.For<IGuildUser>();
        user.JoinedAt.Returns(userJoinedDate);
        user.GuildId.Returns(guildId);
        user.Id.Returns(userId);

        var bot = Substitute.For<IGuildUser>();
        bot.JoinedAt.Returns(botJoinedDate);
        bot.GuildId.Returns(guildId);

        _engagementService.MaxPointsPerDay.Returns(100);
        _engagementService.GetActivityPointsAsync(guildId, userId).Result.Returns((uint)50);

        // Act
        double result = _userModule.GetActivityScoreAsync(user, bot).Result;

        // Assert
        result.ShouldBe(0, 1.0);
    }
}

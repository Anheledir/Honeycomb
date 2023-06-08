using BaseBotService.Commands;
using BaseBotService.Commands.Interfaces;
using BaseBotService.Data.Interfaces;
using Discord;
using Serilog;

namespace BaseBotService.Tests.Commands;

[TestFixture]
public class UserModuleTests
{
    private IEngagementService _engagementService = null!;
    private IMemberRepository _memberRepository = null!;
    private ILogger _logger = null!;
    private UserModule _userModule = null!;
    private readonly Faker _faker = new();

    [SetUp]
    public void SetUp()
    {
        _engagementService = Substitute.For<IEngagementService>();
        _memberRepository = Substitute.For<IMemberRepository>();
        _logger = Substitute.For<ILogger>();

        _userModule = new UserModule(_logger, _engagementService, _memberRepository);
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
        double result = _userModule.GetActivityScore(user, bot);

        // Assert
        result.ShouldBe(0);
    }

    [TestCase((uint)85, 100)]
    [TestCase((uint)125, 100)]
    [TestCase((uint)42, 50)]
    [TestCase((uint)9, 10)]
    public void GetActivityScore_ShouldCalculateCorrectScore_WhenUserAndBotHaveValidJoinedAtDates(uint points, double score)
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
        _engagementService.GetActivityPoints(guildId, userId).Returns(points);

        // Act
        double result = _userModule.GetActivityScore(user, bot);

        // Assert
        result.ShouldBe(score, 1.0);
    }
}
using BaseBotService.Core.Base;
using BaseBotService.Data.Models;
using BaseBotService.Infrastructure;
using Serilog;

namespace BaseBotService.Tests.Infrastructure;

public class AchievementFactoryTests
{
    private IServiceProvider _serviceProvider;
    private ILogger _logger;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _serviceProvider.GetService(typeof(CustomHCAchievement)).Returns(new CustomHCAchievement());
        _serviceProvider.GetService(typeof(ILogger)).Returns(Substitute.For<ILogger>());

        _logger = Substitute.For<ILogger>();
    }

    [Test]
    public void CreateAchievement_ShouldCreateValidAchievement()
    {
        // Arrange

        GuildMemberHC guildMember = FakeDataHelper.GuildFaker.Generate().Members[0];

        // Act
        var achievement = new AchievementFactory(_serviceProvider, _logger).CreateAchievement<CustomHCAchievement>(guildMember);

        // Assert
        achievement.ShouldNotBeNull();
        achievement.MemberId.ShouldBe(guildMember.MemberId);
        achievement.GuildId.ShouldBe(guildMember.GuildId);
        achievement.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
    }

    [Test]
    public void CreateAchievement_WithNoGuildId_ShouldCreateValidGlobalAchievement()
    {
        // Arrange
        MemberHC member = FakeDataHelper.MemberFaker.Generate();

        // Act
        var achievement = new AchievementFactory(_serviceProvider, _logger).CreateAchievement<CustomHCAchievement>(member);

        // Assert
        achievement.ShouldNotBeNull();
        achievement.MemberId.ShouldBe(member.MemberId);
        achievement.GuildId.ShouldBeNull();
        achievement.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
    }
}

public class CustomHCAchievement : AchievementBase
{
    // You can add custom properties or methods for this test class
}

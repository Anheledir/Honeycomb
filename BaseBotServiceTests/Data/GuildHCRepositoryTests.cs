using BaseBotService.Data;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Tests.Data;

[TestFixture]
public class GuildHCRepositoryTests
{
    private ILiteCollection<GuildHC> _guilds;
    private IGuildHCRepository _repository;
    private Faker<GuildHC> _guildFaker;
    private readonly Faker _faker = new();

    [SetUp]
    public void SetUp()
    {
        var db = new LiteDatabase(":memory:");
        _guilds = db.GetCollection<GuildHC>("guilds");

        _repository = new GuildHCRepository(_guilds);

        _guildFaker = new Faker<GuildHC>()
            .RuleFor(g => g.GuildId, f => f.Random.ULong())
            .RuleFor(g => g.ActivityPointsAverageActiveHours, f => f.Random.Int(1, 12))
            .RuleFor(g => g.ActivityPointsName, f => f.Commerce.ProductName())
            .RuleFor(g => g.ActivityPointsSymbol, _ => FakeDataHelper.RandomEmoji())
            .RuleFor(g => g.ArtistRoles, _ => FakeDataHelper.GenerateRandomUlongList())
            .RuleFor(g => g.ModeratorRoles, _ => FakeDataHelper.GenerateRandomUlongList());
    }

    [Test]
    public void GetGuild_WhenGuildExists_ReturnsGuild()
    {
        // Arrange
        var guild = _guildFaker.Generate();
        _guilds.Insert(guild);

        // Act
        var result = _repository.GetGuild(guild.GuildId);

        // Assert
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(guild);
    }

    [Test]
    public void GetGuild_WhenGuildDoesNotExistAndTouchIsFalse_ReturnsNull()
    {
        // Arrange
        ulong guildId = new Faker().Random.ULong();

        // Act
        var result = _repository.GetGuild(guildId);

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public void GetGuild_WhenGuildDoesNotExistAndTouchIsTrue_ReturnsGuild()
    {
        // Arrange
        ulong guildId = new Faker().Random.ULong();

        // Act
        var result = _repository.GetGuild(guildId, touch: true);

        // Assert
        result.ShouldNotBeNull();
        result.GuildId.ShouldBe(guildId);
    }

    [Test]
    public void AddGuild_AddsGuildToCollection()
    {
        // Arrange
        var newGuild = _guildFaker.Generate();

        // Act
        _repository.AddGuild(newGuild);

        // Assert
        var result = _guilds.FindOne(g => g.GuildId == newGuild.GuildId);
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(newGuild);
    }

    [Test]
    public void UpdateGuild_UpdatesGuildInCollection()
    {
        // Arrange
        var existingGuild = _guildFaker.Generate();
        _guilds.Insert(existingGuild);

        existingGuild.ActivityPointsAverageActiveHours += 2;
        existingGuild.ActivityPointsName = _faker.Commerce.ProductName();
        existingGuild.ActivityPointsSymbol = FakeDataHelper.RandomEmoji();
        existingGuild.ArtistRoles = FakeDataHelper.GenerateRandomUlongList();
        existingGuild.ModeratorRoles = FakeDataHelper.GenerateRandomUlongList();

        // Act
        var result = _repository.UpdateGuild(existingGuild);

        // Assert
        result.ShouldBeTrue();
        var updatedGuild = _guilds.FindOne(g => g.GuildId == existingGuild.GuildId);
        updatedGuild.ShouldNotBeNull();
        updatedGuild.Should().BeEquivalentTo(existingGuild);
    }

    [Test]
    public void DeleteGuild_WhenGuildExists_DeletesGuildFromCollection()
    {
        // Arrange
        var existingGuild = _guildFaker.Generate();
        _guilds.Insert(existingGuild);

        // Act
        var result = _repository.DeleteGuild(existingGuild.GuildId);

        // Assert
        result.ShouldBeTrue();
        var deletedGuild = _guilds.FindOne(g => g.GuildId == existingGuild.GuildId);
        deletedGuild.ShouldBeNull();
    }

    [Test]
    public void DeleteGuild_WhenGuildDoesNotExist_ReturnsFalse()
    {
        // Arrange
        ulong guildId = new Faker().Random.ULong();

        // Act
        var result = _repository.DeleteGuild(guildId);

        // Assert
        result.ShouldBeFalse();
    }
}

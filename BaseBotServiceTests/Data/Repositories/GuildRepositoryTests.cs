using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Data.Repositories;
using LiteDB;

namespace BaseBotService.Tests.Data.Repositories;

[TestFixture]
public class GuildRepositoryTests
{
    private ILiteCollection<GuildHC> _guilds;
    private IGuildRepository _repository;
    private readonly Faker _faker = new();

    [SetUp]
    public void SetUp()
    {
        LiteDatabase db = FakeDataHelper.GetTestDatabase();
        _guilds = db.GetCollection<GuildHC>();

        _repository = new GuildRepository(_guilds);
    }

    [Test]
    public void GetGuild_WhenGuildExists_ReturnsGuild()
    {
        // Arrange
        var guild = FakeDataHelper.GuildFaker.Generate();
        _guilds.Insert(guild);

        // Act
        var result = _repository.GetGuild(guild.GuildId);

        // Assert
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(guild, o => o.Excluding(g => g.Members));
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
        var newGuild = FakeDataHelper.GuildFaker.Generate();

        // Act
        _repository.AddGuild(newGuild);

        // Assert
        var result = _guilds.FindOne(g => g.GuildId == newGuild.GuildId);
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(newGuild, o => o.Excluding(g => g.Members));
    }

    [Test]
    public void UpdateGuild_UpdatesGuildInCollection()
    {
        // Arrange
        var existingGuild = FakeDataHelper.GuildFaker.Generate();

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
        updatedGuild.Should().BeEquivalentTo(existingGuild, o => o.Excluding(g => g.Members));
    }

    [Test]
    public void DeleteGuild_WhenGuildExists_DeletesGuildFromCollection()
    {
        // Arrange
        var existingGuild = FakeDataHelper.GuildFaker.Generate();
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

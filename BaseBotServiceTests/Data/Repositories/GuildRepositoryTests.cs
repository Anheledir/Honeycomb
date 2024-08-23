using BaseBotService.Data;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaseBotService.Tests.Data.Repositories;

[TestFixture]
public class GuildRepositoryTests
{
    private DbContextOptions<HoneycombDbContext> _dbContextOptions;
    private HoneycombDbContext _dbContext;
    private IGuildRepository _repository;
    private readonly Faker _faker = new();

    [SetUp]
    public void SetUp()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HoneycombDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _dbContext = new HoneycombDbContext(_dbContextOptions);
        _dbContext.Database.OpenConnection();
        _dbContext.Database.EnsureCreated();

        _repository = new GuildRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetGuild_WhenGuildExists_ReturnsGuild()
    {
        // Arrange
        var guild = FakeDataHelper.GuildFaker.Generate();
        _dbContext.Guilds.Add(guild);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetGuildAsync(guild.GuildId);

        // Assert
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(guild, o => o.Excluding(g => g.Members));
    }

    [Test]
    public async Task GetGuild_WhenGuildDoesNotExistAndTouchIsFalse_ReturnsNull()
    {
        // Arrange
        ulong guildId = _faker.Random.ULong();

        // Act
        var result = await _repository.GetGuildAsync(guildId);

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public async Task GetGuild_WhenGuildDoesNotExistAndTouchIsTrue_ReturnsGuild()
    {
        // Arrange
        ulong guildId = _faker.Random.ULong();

        // Act
        var result = await _repository.GetGuildAsync(guildId, create: true);

        // Assert
        result.ShouldNotBeNull();
        result.GuildId.ShouldBe(guildId);
    }

    [Test]
    public async Task AddGuild_AddsGuildToCollection()
    {
        // Arrange
        var newGuild = FakeDataHelper.GuildFaker.Generate();

        // Act
        await _repository.AddGuildAsync(newGuild);

        // Assert
        var result = await _dbContext.Guilds.FirstOrDefaultAsync(g => g.GuildId == newGuild.GuildId);
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(newGuild, o => o.Excluding(g => g.Members));
    }

    [Test]
    public async Task UpdateGuild_UpdatesGuildInCollection()
    {
        // Arrange
        var existingGuild = FakeDataHelper.GuildFaker.Generate();
        _dbContext.Guilds.Add(existingGuild);
        await _dbContext.SaveChangesAsync();

        existingGuild.ActivityPointsAverageActiveHours += 2;
        existingGuild.ActivityPointsName = _faker.Commerce.ProductName();
        existingGuild.ActivityPointsSymbol = FakeDataHelper.RandomEmoji();
        existingGuild.ArtistRoles = FakeDataHelper.GenerateRandomUlongList();
        existingGuild.ModeratorRoles = FakeDataHelper.GenerateRandomUlongList();

        // Act
        var result = await _repository.UpdateGuildAsync(existingGuild);

        // Assert
        result.ShouldBeTrue();
        var updatedGuild = await _dbContext.Guilds.FirstOrDefaultAsync(g => g.GuildId == existingGuild.GuildId);
        updatedGuild.ShouldNotBeNull();
        updatedGuild.Should().BeEquivalentTo(existingGuild, o => o.Excluding(g => g.Members));
    }

    [Test]
    public async Task DeleteGuild_WhenGuildExists_DeletesGuildFromCollection()
    {
        // Arrange
        var existingGuild = FakeDataHelper.GuildFaker.Generate();
        _dbContext.Guilds.Add(existingGuild);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteGuildAsync(existingGuild.GuildId);

        // Assert
        result.ShouldBeTrue();
        var deletedGuild = await _dbContext.Guilds.FirstOrDefaultAsync(g => g.GuildId == existingGuild.GuildId);
        deletedGuild.ShouldBeNull();
    }

    [Test]
    public async Task DeleteGuild_WhenGuildDoesNotExist_ReturnsFalse()
    {
        // Arrange
        ulong guildId = _faker.Random.ULong();

        // Act
        var result = await _repository.DeleteGuildAsync(guildId);

        // Assert
        result.ShouldBeFalse();
    }
}

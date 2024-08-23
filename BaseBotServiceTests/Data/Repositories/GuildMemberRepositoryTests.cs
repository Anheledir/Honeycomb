using BaseBotService.Data;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaseBotService.Tests.Data.Repositories;

[TestFixture]
public class GuildMemberRepositoryTests
{
    private DbContextOptions<HoneycombDbContext> _dbContextOptions;
    private HoneycombDbContext _dbContext;
    private IGuildMemberRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _dbContextOptions = new DbContextOptionsBuilder<HoneycombDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _dbContext = new HoneycombDbContext(_dbContextOptions);
        _dbContext.Database.OpenConnection();
        _dbContext.Database.EnsureCreated();

        _repository = new GuildMemberRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();  // Properly dispose of the DbContext to prevent memory leaks
    }

    [Test]
    public async Task GetUser_ShouldReturnCorrectUser()
    {
        // Arrange
        var guild = FakeDataHelper.GuildFaker.Generate();
        var guildMember = guild.Members[0];
        _dbContext.GuildMembers.Add(guildMember);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserAsync(guildMember.GuildId, guildMember.MemberId);

        // Assert
        result.ShouldNotBeNull();
        result.MemberId.ShouldBe(guildMember.MemberId);
        result.GuildId.ShouldBe(guildMember.GuildId);
    }

    [Test]
    public async Task AddUser_ShouldAddNewUser()
    {
        // Arrange
        var guild = FakeDataHelper.GuildFaker.Generate();
        var newUser = guild.Members[0];
        _dbContext.Guilds.Add(guild);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.AddUserAsync(newUser);

        // Assert
        var result = await _dbContext.GuildMembers
            .FirstOrDefaultAsync(u => u.MemberId == newUser.MemberId && u.GuildId == newUser.GuildId);
        result.ShouldNotBeNull();
        result.MemberId.ShouldBe(newUser.MemberId);
        result.GuildId.ShouldBe(newUser.GuildId);
    }

    [Test]
    public async Task UpdateUser_ShouldUpdateExistingUser()
    {
        // Arrange
        var existingUser = FakeDataHelper.GuildFaker.Generate().Members[0];
        _dbContext.GuildMembers.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        existingUser.ActivityPoints++;

        // Act
        var updateResult = await _repository.UpdateUserAsync(existingUser);

        // Assert
        updateResult.ShouldBeTrue();
        var updatedUser = await _dbContext.GuildMembers
            .FirstOrDefaultAsync(u => u.MemberId == existingUser.MemberId && u.GuildId == existingUser.GuildId);
        updatedUser.ShouldNotBeNull();
        updatedUser.ActivityPoints.ShouldBe(existingUser.ActivityPoints);
    }

    [Test]
    public async Task DeleteUser_ShouldDeleteExistingUser()
    {
        // Arrange
        var existingUser = FakeDataHelper.GuildFaker.Generate().Members[0];
        _dbContext.GuildMembers.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var deleteResult = await _repository.DeleteUserAsync(existingUser.GuildId, existingUser.MemberId);

        // Assert
        deleteResult.ShouldBeTrue();
        var deletedUser = await _dbContext.GuildMembers
            .FirstOrDefaultAsync(u => u.MemberId == existingUser.MemberId && u.GuildId == existingUser.GuildId);
        deletedUser.ShouldBeNull();
    }
}

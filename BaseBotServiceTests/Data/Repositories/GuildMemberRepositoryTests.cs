using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Data.Repositories;
using LiteDB;

namespace BaseBotService.Tests.Data.Repositories;

[TestFixture]
public class GuildMemberRepositoryTests
{
    private ILiteCollection<GuildMemberHC> _guildMembers;
    private ILiteCollection<GuildHC> _guilds;
    private IGuildMemberRepository _repository;

    [SetUp]
    public void SetUp()
    {
        LiteDatabase db = FakeDataHelper.GetTestDatabase();
        _guildMembers = db.GetCollection<GuildMemberHC>();
        _guilds = db.GetCollection<GuildHC>();
        _repository = new GuildMemberRepository(_guildMembers);
    }

    [Test]
    public void GetUser_ShouldReturnCorrectUser()
    {
        // Arrange
        var guild = FakeDataHelper.GuildFaker.Generate();
        var guildMember = guild.Members[0];
        _guildMembers.Insert(guildMember);

        // Act
        var result = _repository.GetUser(guildMember.GuildId, guildMember.MemberId);

        // Assert
        result.ShouldNotBeNull();
        result.MemberId.ShouldBe(guildMember.MemberId);
        result.GuildId.ShouldBe(guildMember.GuildId);
    }

    [Test]
    public void AddUser_ShouldAddNewUser()
    {
        // Arrange
        var guild = FakeDataHelper.GuildFaker.Generate();
        var newUser = guild.Members[0];
        _guilds.Insert(guild);

        // Act
        _repository.AddUser(newUser);

        // Assert
        var result = _guildMembers
            .FindOne(u => u.MemberId == newUser.MemberId && u.GuildId == newUser.GuildId);
        result.ShouldNotBeNull();
        result.MemberId.ShouldBe(newUser.MemberId);
        result.GuildId.ShouldBe(newUser.GuildId);
    }

    [Test]
    public void UpdateUser_ShouldUpdateExistingUser()
    {
        // Arrange
        var existingUser = FakeDataHelper.GuildFaker.Generate().Members[0];
        _guildMembers.Insert(existingUser);

        existingUser.ActivityPoints++;

        // Act
        var updateResult = _repository.UpdateUser(existingUser);

        // Assert
        updateResult.ShouldBeTrue();
        var updatedUser = _guildMembers.FindOne(u => u.MemberId == existingUser.MemberId && u.GuildId == existingUser.GuildId);
        updatedUser.ShouldNotBeNull();
        updatedUser.ActivityPoints.ShouldBe(existingUser.ActivityPoints);
    }

    [Test]
    public void DeleteUser_ShouldDeleteExistingUser()
    {
        // Arrange
        var existingUser = FakeDataHelper.GuildFaker.Generate().Members[0];
        _guildMembers.Insert(existingUser);

        // Act
        var deleteResult = _repository.DeleteUser(existingUser.GuildId, existingUser.MemberId);

        // Assert
        deleteResult.ShouldBeTrue();
        var deletedUser = _guildMembers.FindOne(u => u.MemberId == existingUser.MemberId && u.GuildId == existingUser.GuildId);
        deletedUser.ShouldBeNull();
    }
}

using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Data.Repositories;
using LiteDB;

namespace BaseBotService.Tests.Data.Repositories;

[TestFixture]
public class GuildMemberRepositoryTests
{
    private ILiteCollection<GuildMemberHC> _guildMembers;
    private IGuildMemberRepository _repository;
    private Faker<GuildMemberHC> _guildMemberFaker;

    [SetUp]
    public void SetUp()
    {
        var db = new LiteDatabase(":memory:");
        _guildMembers = db.GetCollection<GuildMemberHC>("guildMembers");

        _repository = new GuildMemberRepository(_guildMembers);

        _guildMemberFaker = new Faker<GuildMemberHC>()
            .RuleFor(u => u.MemberId, f => f.Random.ULong())
            .RuleFor(u => u.GuildId, f => f.Random.ULong())
            .RuleFor(u => u.ActivityPoints, f => f.Random.UInt())
            .RuleFor(u => u.LastActive, f => f.Date.Recent())
            .RuleFor(u => u.LastActivityPoint, f => f.Date.Recent());
    }

    [Test]
    public void GetUser_ShouldReturnCorrectUser()
    {
        // Arrange
        var guildMember = _guildMemberFaker.Generate();
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
        var newUser = _guildMemberFaker.Generate();

        // Act
        _repository.AddUser(newUser);

        // Assert
        var result = _guildMembers.FindOne(u => u.MemberId == newUser.MemberId && u.GuildId == newUser.GuildId);
        result.ShouldNotBeNull();
        result.MemberId.ShouldBe(newUser.MemberId);
        result.GuildId.ShouldBe(newUser.GuildId);
    }

    [Test]
    public void UpdateUser_ShouldUpdateExistingUser()
    {
        // Arrange
        var existingUser = _guildMemberFaker.Generate();
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
        var existingUser = _guildMemberFaker.Generate();
        _guildMembers.Insert(existingUser);

        // Act
        var deleteResult = _repository.DeleteUser(existingUser.GuildId, existingUser.MemberId);

        // Assert
        deleteResult.ShouldBeTrue();
        var deletedUser = _guildMembers.FindOne(u => u.MemberId == existingUser.MemberId && u.GuildId == existingUser.GuildId);
        deletedUser.ShouldBeNull();
    }
}

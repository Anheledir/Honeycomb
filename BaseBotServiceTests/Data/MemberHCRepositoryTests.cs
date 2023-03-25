using BaseBotService.Data;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Interactions.Enums;
using LiteDB;

namespace BaseBotService.Tests.Data;

[TestFixture]
public class MemberHCRepositoryTests
{
    private ILiteCollection<MemberHC> _members;
    private IMemberHCRepository _repository;
    private Faker<MemberHC> _memberFaker;

    [SetUp]
    public void SetUp()
    {
        var db = new LiteDatabase(":memory:");
        _members = db.GetCollection<MemberHC>("members");

        _repository = new MemberHCRepository(_members);

        _memberFaker = new Faker<MemberHC>()
            .RuleFor(u => u.MemberId, f => f.Random.ULong())
            .RuleFor(u => u.Timezone, f => f.Random.Enum<Timezone>())
            .RuleFor(u => u.Country, f => f.Random.Enum<Countries>())
            .RuleFor(u => u.Languages, f => f.Random.Enum<Languages>())
            .RuleFor(u => u.Birthday, f => f.Date.Between(DateTime.Today.AddYears(-50), DateTime.Today.AddYears(-13)))
            .RuleFor(u => u.GenderIdentity, f => f.Random.Enum<GenderIdentity>());
    }

    [Test]
    public void GetUser_ShouldReturnCorrectUser()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _members.Insert(member);

        // Act
        var result = _repository.GetUser(member.MemberId);

        // Assert
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(member, options => options
        // allow rounding errors on comparing times
        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
        .WhenTypeIs<DateTime>());
    }

    [Test]
    public void AddUser_ShouldAddNewUser()
    {
        // Arrange
        var newUser = _memberFaker.Generate();

        // Act
        _repository.AddUser(newUser);

        // Assert
        var result = _members.FindOne(u => u.MemberId == newUser.MemberId);
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(newUser, options => options
        // allow rounding errors on comparing times
        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
        .WhenTypeIs<DateTime>());
    }

    [Test]
    public void UpdateUser_ShouldUpdateExistingUser()
    {
        // Arrange
        var existingUser = _memberFaker.Generate();
        _members.Insert(existingUser);

        existingUser.Timezone += 1;

        // Act
        var updateResult = _repository.UpdateUser(existingUser);

        // Assert
        updateResult.ShouldBeTrue();
        var updatedUser = _members.FindOne(u => u.MemberId == existingUser.MemberId);
        updatedUser.ShouldNotBeNull();
        updatedUser.Timezone.ShouldBe(existingUser.Timezone);
    }

    [Test]
    public void DeleteUser_ShouldDeleteExistingUser()
    {
        // Arrange
        var existingUser = _memberFaker.Generate();
        _members.Insert(existingUser);

        // Act
        var deleteResult = _repository.DeleteUser(existingUser.MemberId);

        // Assert
        deleteResult.ShouldBeTrue();
        var deletedUser = _members.FindOne(u => u.MemberId == existingUser.MemberId);
        deletedUser.ShouldBeNull();
    }
}

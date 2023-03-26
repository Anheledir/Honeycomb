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
    public void GetUser_WhenUserExists_ShouldReturnCorrectUser()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _members.Insert(member);

        // Act
        var result = _repository.GetUser(member.MemberId, false);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(member, options => options
        // allow rounding errors on comparing times
        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
        .WhenTypeIs<DateTime>());
    }

    [Test]
    public void GetUser_WhenUserDoesNotExistAndTouchIsFalse_ShouldReturnNull()
    {
        // Arrange
        const ulong nonExistentUserId = 12345;

        // Act
        var result = _repository.GetUser(nonExistentUserId, false);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GetUser_WhenUserDoesNotExistAndTouchIsTrue_ShouldCreateAndReturnNewUser()
    {
        // Arrange
        const ulong nonExistentUserId = 12345;

        // Act
        var result = _repository.GetUser(nonExistentUserId, true);

        // Assert
        result.Should().NotBeNull();
        result.MemberId.Should().Be(nonExistentUserId);

        var createdUser = _members.FindOne(a => a.MemberId == nonExistentUserId);
        createdUser.Should().NotBeNull();
        createdUser.Should().BeEquivalentTo(result, options => options
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

        existingUser.Timezone += 60;
        existingUser.Country++;
        existingUser.Languages++;
        existingUser.GenderIdentity++;
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        existingUser.Birthday += TimeSpan.FromDays(1);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

        // Act
        var updateResult = _repository.UpdateUser(existingUser);

        // Assert
        updateResult.ShouldBeTrue();
        var updatedUser = _members.FindOne(u => u.MemberId == existingUser.MemberId);
        updatedUser.ShouldNotBeNull();
        updatedUser.Should().BeEquivalentTo(existingUser, options => options
        // allow rounding errors on comparing times
        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
        .WhenTypeIs<DateTime>());
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

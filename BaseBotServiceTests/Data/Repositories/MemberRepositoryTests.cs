﻿using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Data.Repositories;
using LiteDB;

namespace BaseBotService.Tests.Data.Repositories;

[TestFixture]
public class MemberRepositoryTests
{
    private ILiteCollection<MemberHC> _members;
    private IMemberRepository _repository;
    private readonly Faker _faker = new();

    [SetUp]
    public void SetUp()
    {
        LiteDatabase db = FakeDataHelper.GetTestDatabase();
        _members = db.GetCollection<MemberHC>();

        _repository = new MemberRepository(_members);
    }

    [Test]
    public void GetUser_WhenUserExists_ShouldReturnCorrectUser()
    {
        // Arrange
        var member = FakeDataHelper.MemberFaker.Generate();
        _members.Insert(member);

        // Act
        var result = _repository.GetUser(member.MemberId, false);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(member, options => options
        .Excluding(u => u.Achievements)
        // allow rounding errors on comparing times
        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
        .WhenTypeIs<DateTime>());
    }

    [Test]
    public void GetUser_WhenUserDoesNotExistAndTouchIsFalse_ShouldReturnNull()
    {
        // Arrange
        ulong nonExistentUserId = _faker.Random.ULong();

        // Act
        var result = _repository.GetUser(nonExistentUserId, false);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GetUser_WhenUserDoesNotExistAndTouchIsTrue_ShouldCreateAndReturnNewUser()
    {
        // Arrange
        ulong nonExistentUserId = _faker.Random.ULong();

        // Act
        var result = _repository.GetUser(nonExistentUserId, true);

        // Assert
        result.Should().NotBeNull();
        result?.MemberId.Should().Be(nonExistentUserId);

        var createdUser = _members.FindOne(a => a.MemberId == nonExistentUserId);
        createdUser.Should().NotBeNull();
        createdUser.Should().BeEquivalentTo(result, options => options
        .Excluding(u => u!.Achievements)
        // allow rounding errors on comparing times
        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
        .WhenTypeIs<DateTime>());
    }

    [Test]
    public void AddUser_ShouldAddNewUser()
    {
        // Arrange
        var newUser = FakeDataHelper.MemberFaker.Generate();

        // Act
        _repository.AddUser(newUser);

        // Assert
        var result = _members.FindOne(u => u.MemberId == newUser.MemberId);
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(newUser, options => options
        .Excluding(u => u.Achievements)
        // allow rounding errors on comparing times
        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
        .WhenTypeIs<DateTime>());
    }

    [Test]
    public void UpdateUser_ShouldUpdateExistingUser()
    {
        // Arrange
        var existingUser = FakeDataHelper.MemberFaker.Generate();
        _members.Insert(existingUser);

        existingUser.Timezone += 60;
        existingUser.Country++;
        existingUser.Languages++;
        existingUser.GenderIdentity++;
        existingUser.Birthday = existingUser.Birthday! + TimeSpan.FromDays(1);

        // Act
        var updateResult = _repository.UpdateUser(existingUser);

        // Assert
        updateResult.ShouldBeTrue();
        var updatedUser = _members.FindOne(u => u.MemberId == existingUser.MemberId);
        updatedUser.ShouldNotBeNull();
        updatedUser.Should().BeEquivalentTo(existingUser, options => options
        .Excluding(u => u.Achievements)
        // allow rounding errors on comparing times
        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
        .WhenTypeIs<DateTime>());
    }

    [Test]
    public void DeleteUser_ShouldDeleteExistingUser()
    {
        // Arrange
        var existingUser = FakeDataHelper.MemberFaker.Generate();
        _members.Insert(existingUser);

        // Act
        var deleteResult = _repository.DeleteUser(existingUser.MemberId);

        // Assert
        deleteResult.ShouldBeTrue();
        var deletedUser = _members.FindOne(u => u.MemberId == existingUser.MemberId);
        deletedUser.ShouldBeNull();
    }
}

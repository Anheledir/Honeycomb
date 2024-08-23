using BaseBotService.Data;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaseBotService.Tests.Data.Repositories;

[TestFixture]
public class MemberRepositoryTests
{
    private DbContextOptions<HoneycombDbContext> _dbContextOptions;
    private HoneycombDbContext _dbContext;
    private IMemberRepository _repository;
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

        _repository = new MemberRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();  // Properly dispose of the DbContext to prevent memory leaks
    }

    [Test]
    public async Task GetUser_WhenUserExists_ShouldReturnCorrectUser()
    {
        // Arrange
        var member = FakeDataHelper.MemberFaker.Generate();
        _dbContext.Members.Add(member);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserAsync(member.MemberId, false);

        // Assert
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(member, options => options
            .Excluding(u => u.Achievements)
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Test]
    public async Task GetUser_WhenUserDoesNotExistAndTouchIsFalse_ShouldReturnNull()
    {
        // Arrange
        ulong nonExistentUserId = _faker.Random.ULong();

        // Act
        var result = await _repository.GetUserAsync(nonExistentUserId, false);

        // Assert
        result.ShouldBeNull();
    }

    [Test]
    public async Task GetUser_WhenUserDoesNotExistAndTouchIsTrue_ShouldCreateAndReturnNewUser()
    {
        // Arrange
        ulong nonExistentUserId = _faker.Random.ULong();

        // Act
        var result = await _repository.GetUserAsync(nonExistentUserId, true);

        // Assert
        result.ShouldNotBeNull();
        result?.MemberId.Should().Be(nonExistentUserId);

        var createdUser = await _dbContext.Members.FindAsync(nonExistentUserId);
        createdUser.ShouldNotBeNull();
        createdUser.Should().BeEquivalentTo(result, options => options
            .Excluding(u => u.Achievements)
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Test]
    public async Task AddUser_ShouldAddNewUser()
    {
        // Arrange
        var newUser = FakeDataHelper.MemberFaker.Generate();

        // Act
        await _repository.AddUserAsync(newUser);

        // Assert
        var result = await _dbContext.Members.FirstOrDefaultAsync(u => u.MemberId == newUser.MemberId);
        result.ShouldNotBeNull();
        result.Should().BeEquivalentTo(newUser, options => options
            .Excluding(u => u.Achievements)
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Test]
    public async Task UpdateUser_ShouldUpdateExistingUser()
    {
        // Arrange
        var existingUser = FakeDataHelper.MemberFaker.Generate();
        _dbContext.Members.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        existingUser.Timezone += 60;
        existingUser.Country++;
        existingUser.Languages++;
        existingUser.GenderIdentity++;
        existingUser.Birthday = existingUser.Birthday?.AddDays(1);

        // Act
        var updateResult = await _repository.UpdateUserAsync(existingUser);

        // Assert
        updateResult.ShouldBeTrue();
        var updatedUser = await _dbContext.Members.FirstOrDefaultAsync(u => u.MemberId == existingUser.MemberId);
        updatedUser.ShouldNotBeNull();
        updatedUser.Should().BeEquivalentTo(existingUser, options => options
            .Excluding(u => u.Achievements)
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Test]
    public async Task DeleteUser_ShouldDeleteExistingUser()
    {
        // Arrange
        var existingUser = FakeDataHelper.MemberFaker.Generate();
        _dbContext.Members.Add(existingUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var deleteResult = await _repository.DeleteUserAsync(existingUser.MemberId);

        // Assert
        deleteResult.ShouldBeTrue();
        var deletedUser = await _dbContext.Members.FirstOrDefaultAsync(u => u.MemberId == existingUser.MemberId);
        deletedUser.ShouldBeNull();
    }
}

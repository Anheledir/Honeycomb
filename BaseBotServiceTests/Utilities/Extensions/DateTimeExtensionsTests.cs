using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities.Enums;
using BaseBotService.Utilities.Extensions;

namespace BaseBotService.Tests.Utilities.Extensions;

public class DateTimeExtensionsTests
{
    private readonly Faker _faker;

    public DateTimeExtensionsTests()
    {
        _faker = new Faker();
    }

    [Test]
    public void ToUnixTimestamp_ConvertsDateTimeToUnixTimestamp()
    {
        // Arrange
        DateTime dateTime = _faker.Date.Recent();

        // Act
        long unixTimestamp = dateTime.ToUnixTimestamp();

        // Assert
        long expectedTimestamp = (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        unixTimestamp.ShouldBe(expectedTimestamp);
    }

    [Test]
    public void ToUnixTimestamp_ConvertsDateTimeOffsetToUnixTimestamp()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = _faker.Date.RecentOffset();

        // Act
        long unixTimestamp = dateTimeOffset.ToUnixTimestamp();

        // Assert
        long expectedTimestamp = (long)(dateTimeOffset - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
        unixTimestamp.ShouldBe(expectedTimestamp);
    }

    [TestCase(DiscordTimestampFormat.ShortTime)]
    [TestCase(DiscordTimestampFormat.LongTime)]
    [TestCase(DiscordTimestampFormat.ShortDate)]
    [TestCase(DiscordTimestampFormat.LongDate)]
    [TestCase(DiscordTimestampFormat.ShortDateTime)]
    [TestCase(DiscordTimestampFormat.LongDateTime)]
    [TestCase(DiscordTimestampFormat.RelativeTime)]
    public void ToDiscordTimestamp_FormatsDateTimeAsDiscordTimestamp(DiscordTimestampFormat format)
    {
        // Arrange
        DateTime dateTime = _faker.Date.Recent();
        var translationService = Substitute.For<ITranslationService>();
        translationService.GetString("not-available").Returns("n/a");

        // Act
        string discordTimestamp = dateTime.ToDiscordTimestamp(translationService, format);

        // Assert
        string expectedTimestamp = $"<t:{dateTime.ToUnixTimestamp()}:{format.DiscordTimestampFormatHelper()}>";
        discordTimestamp.ShouldBe(expectedTimestamp);
    }

    [TestCase(DiscordTimestampFormat.ShortTime)]
    [TestCase(DiscordTimestampFormat.LongTime)]
    [TestCase(DiscordTimestampFormat.ShortDate)]
    [TestCase(DiscordTimestampFormat.LongDate)]
    [TestCase(DiscordTimestampFormat.ShortDateTime)]
    [TestCase(DiscordTimestampFormat.LongDateTime)]
    [TestCase(DiscordTimestampFormat.RelativeTime)]
    public void ToDiscordTimestamp_FormatsDateTimeOffsetAsDiscordTimestamp(DiscordTimestampFormat format)
    {
        // Arrange
        DateTimeOffset dateTimeOffset = _faker.Date.RecentOffset();
        var translationService = Substitute.For<ITranslationService>();
        translationService.GetString("not-available").Returns("n/a");

        // Act
        string discordTimestamp = dateTimeOffset.ToDiscordTimestamp(translationService, format);

        // Assert
        string expectedTimestamp = $"<t:{dateTimeOffset.ToUnixTimestamp()}:{format.DiscordTimestampFormatHelper()}>";
        discordTimestamp.ShouldBe(expectedTimestamp);
    }

    [Test]
    public void ToDiscordTimestamp_MinDateTime_ReturnsNotAvailable()
    {
        // Arrange
        DateTime dateTime = DateTime.MinValue;
        var translationService = Substitute.For<ITranslationService>();
        translationService.GetString("not-available").Returns("n/a");

        // Act
        string discordTimestamp = dateTime.ToDiscordTimestamp(translationService);

        // Assert
        discordTimestamp.ShouldBe("n/a");
    }

    [Test]
    public void ToDiscordTimestamp_MinDateTimeOffset_ReturnsNotAvailable()
    {
        // Arrange
        DateTimeOffset dateTimeOffset = DateTimeOffset.MinValue;
        var translationService = Substitute.For<ITranslationService>();
        translationService.GetString("not-available").Returns("n/a");

        // Act
        string discordTimestamp = dateTimeOffset.ToDiscordTimestamp(translationService);

        // Assert
        discordTimestamp.ShouldBe("n/a");
    }

    private readonly DateTime _dateTime = new(2022, 3, 17, 12, 0, 0, DateTimeKind.Utc);

    [TestCase(Timezone.GMT, "2022-03-17T12:00:00")]
    [TestCase(Timezone.PST, "2022-03-17T04:00:00")]
    [TestCase(Timezone.EST, "2022-03-17T07:00:00")]
    [TestCase(Timezone.CET, "2022-03-17T13:00:00")]
    public void ToTimezone_ShouldReturnCorrectDateTime(Timezone timezone, string expectedDateTimeString)
    {
        // Arrange
        DateTime expectedDateTime = DateTime.Parse(expectedDateTimeString);

        // Act
        DateTime actualDateTime = _dateTime.ToTimezone(timezone);

        // Assert
        actualDateTime.ShouldBe(expectedDateTime, TimeSpan.FromMinutes(1));
    }

    [TestCase(Timezone.GMT, "2022-03-17T12:00:00")]
    [TestCase(Timezone.PST, "2022-03-17T20:00:00")]
    [TestCase(Timezone.EST, "2022-03-17T17:00:00")]
    [TestCase(Timezone.CET, "2022-03-17T11:00:00")]
    public void ToUtcFromTimezone_ShouldReturnCorrectDateTime(Timezone timezone, string expectedDateTimeString)
    {
        // Arrange
        DateTime expectedDateTime = DateTime.Parse(expectedDateTimeString);

        // Act
        DateTime actualDateTime = _dateTime.ToUtcFromTimezone(timezone);

        // Assert
        actualDateTime.ShouldBe(expectedDateTime, TimeSpan.FromMinutes(1));
    }
}

using BaseBotService.Extensions;
using BaseBotService.Enumeration;

namespace BaseBotService.Tests.Extensions
{
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
            var dateTime = _faker.Date.Recent();

            // Act
            var unixTimestamp = dateTime.ToUnixTimestamp();

            // Assert
            var expectedTimestamp = (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            unixTimestamp.ShouldBe(expectedTimestamp);
        }

        [Test]
        public void ToUnixTimestamp_ConvertsDateTimeOffsetToUnixTimestamp()
        {
            // Arrange
            var dateTimeOffset = _faker.Date.RecentOffset();

            // Act
            var unixTimestamp = dateTimeOffset.ToUnixTimestamp();

            // Assert
            var expectedTimestamp = (long)(dateTimeOffset - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
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
            var dateTime = _faker.Date.Recent();

            // Act
            var discordTimestamp = dateTime.ToDiscordTimestamp(format);

            // Assert
            var expectedTimestamp = $"<t:{dateTime.ToUnixTimestamp()}:{format.DiscordTimestampFormatHelper()}>";
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
            var dateTimeOffset = _faker.Date.RecentOffset();

            // Act
            var discordTimestamp = dateTimeOffset.ToDiscordTimestamp(format);

            // Assert
            var expectedTimestamp = $"<t:{dateTimeOffset.ToUnixTimestamp()}:{format.DiscordTimestampFormatHelper()}>";
            discordTimestamp.ShouldBe(expectedTimestamp);
        }

        [Test]
        public void ToDiscordTimestamp_MinDateTime_ReturnsNotAvailable()
        {
            // Arrange
            var dateTime = DateTime.MinValue;

            // Act
            var discordTimestamp = dateTime.ToDiscordTimestamp();

            // Assert
            discordTimestamp.ShouldBe("n/a");
        }

        [Test]
        public void ToDiscordTimestamp_MinDateTimeOffset_ReturnsNotAvailable()
        {
            // Arrange
            var dateTimeOffset = DateTimeOffset.MinValue;

            // Act
            var discordTimestamp = dateTimeOffset.ToDiscordTimestamp();

            // Assert
            discordTimestamp.ShouldBe("n/a");
        }
    }
}

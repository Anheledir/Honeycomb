using BaseBotService.Extensions;
using Discord;
using Serilog.Events;

namespace BaseBotService.Tests.Extensions
{
    public class DiscordExtensionsTests
    {
        [TestCase(LogSeverity.Critical, LogEventLevel.Fatal)]
        [TestCase(LogSeverity.Error, LogEventLevel.Error)]
        [TestCase(LogSeverity.Warning, LogEventLevel.Warning)]
        [TestCase(LogSeverity.Info, LogEventLevel.Information)]
        [TestCase(LogSeverity.Verbose, LogEventLevel.Verbose)]
        [TestCase(LogSeverity.Debug, LogEventLevel.Debug)]
        public void GetSerilogSeverity_ReturnsCorrectSerilogSeverity(LogSeverity discordSeverity, LogEventLevel expectedSerilogSeverity)
        {
            // Arrange
            var logMessage = new LogMessage(discordSeverity, "Source", "Message");

            // Act
            var serilogSeverity = logMessage.GetSerilogSeverity();

            // Assert
            serilogSeverity.ShouldBe(expectedSerilogSeverity);
        }

        [Test]
        public void GetSerilogSeverity_InvalidLogSeverity_ReturnsVerbose()
        {
            // Arrange
            var invalidSeverity = (LogSeverity)(-1);
            var logMessage = new LogMessage(invalidSeverity, "Source", "Message");

            // Act
            var serilogSeverity = logMessage.GetSerilogSeverity();

            // Assert
            serilogSeverity.ShouldBe(LogEventLevel.Verbose);
        }
    }
}

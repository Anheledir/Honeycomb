using BaseBotService.Utilities.Extensions;
using Discord;
using Serilog.Events;

namespace BaseBotService.Tests.Utilities.Extensions;

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
        LogMessage logMessage = new(discordSeverity, "Source", "Message");

        // Act
        LogEventLevel serilogSeverity = logMessage.GetSerilogSeverity();

        // Assert
        serilogSeverity.ShouldBe(expectedSerilogSeverity);
    }

    [Test]
    public void GetSerilogSeverity_InvalidLogSeverity_ReturnsVerbose()
    {
        // Arrange
        const LogSeverity invalidSeverity = (LogSeverity)(-1);
        LogMessage logMessage = new(invalidSeverity, "Source", "Message");

        // Act
        LogEventLevel serilogSeverity = logMessage.GetSerilogSeverity();

        // Assert
        serilogSeverity.ShouldBe(LogEventLevel.Verbose);
    }
    [Test]
    public void WithFieldIf_ConditionTrue_FieldIsAdded()
    {
        // Arrange
        EmbedBuilder embedBuilder = new();
        EmbedFieldBuilder embedFieldBuilder = new EmbedFieldBuilder().WithName("Name").WithValue("Value");
        const bool condition = true;

        // Act
        _ = embedBuilder.WithFieldIf(condition, embedFieldBuilder);

        // Assert
        embedBuilder.Fields.Count.ShouldBe(1);
        embedBuilder.Fields[0].Name.ShouldBe("Name");
        embedBuilder.Fields[0].Value.ShouldBe("Value");
    }

    [Test]
    public void WithFieldIf_ConditionFalse_FieldIsNotAdded()
    {
        // Arrange
        EmbedBuilder embedBuilder = new();
        EmbedFieldBuilder embedFieldBuilder = new EmbedFieldBuilder().WithName("Name").WithValue("Value");
        const bool condition = false;

        // Act
        _ = embedBuilder.WithFieldIf(condition, embedFieldBuilder);

        // Assert
        embedBuilder.Fields.Count.ShouldBe(0);
    }
}

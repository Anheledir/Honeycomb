using Serilog.Events;

namespace BaseBotService.Utilities.Extensions;

public static class DiscordExtensions
{
    /// <summary>
    /// Map a discord log-level to the corresponding serilog log-level.
    /// </summary>
    /// <param name="logMessage">The origin discord log-message.</param>
    /// <returns>The mapped serilog log-severity.</returns>
    public static LogEventLevel GetSerilogSeverity(this LogMessage logMessage)
    {
        return logMessage.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Verbose,
        };
    }

    public static EmbedBuilder WithFieldIf(this EmbedBuilder builder, bool condition, EmbedFieldBuilder item)
    {
        if (condition)
        {
            _ = builder.WithFields(item);
        }
        return builder;
    }

    public static SelectMenuBuilder AddOptionsFromEnum<T>(
        this SelectMenuBuilder builder,
        int currentValue,
        Func<T, string> getLabel
    ) where T : Enum
    {
        foreach (T value in Enum.GetValues(typeof(T)).Cast<T>())
        {
            string label = getLabel(value).ExtractEmoji(out Emoji emote);
            SelectMenuOptionBuilder option = new SelectMenuOptionBuilder()
                .WithLabel(label)
                .WithValue(Convert.ToInt32(value).ToString())
                .WithDefault(currentValue == Convert.ToInt32(value))
                .WithEmote(emote);
            _ = builder.AddOption(option);
        }
        return builder;
    }
}
using Discord;
using Serilog.Events;

namespace BaseBotService.Extensions;

public static class DiscordExtensions
{

    /// <summary>
    /// Map a discord log-level to the corresponding serilog log-level.
    /// </summary>
    /// <param name="logMessage">The origin discord log-message.</param>
    /// <returns>The mapped serilog log-severity.</returns>
    public static LogEventLevel GetSerilogSeverity(this LogMessage logMessage)
    {
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
                return LogEventLevel.Fatal;
            case LogSeverity.Error:
                return LogEventLevel.Error;
            case LogSeverity.Warning:
                return LogEventLevel.Warning;
            case LogSeverity.Info:
                return LogEventLevel.Information;
            case LogSeverity.Verbose:
                return LogEventLevel.Verbose;
            case LogSeverity.Debug:
                return LogEventLevel.Debug;
            default:
                return LogEventLevel.Verbose;
        }
    }
}
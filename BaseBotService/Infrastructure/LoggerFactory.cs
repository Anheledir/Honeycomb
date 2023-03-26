using BaseBotService.Utilities.Extensions;
using Serilog.Events;

namespace BaseBotService.Infrastructure;

public static class LoggerFactory
{
    public static ILogger CreateLogger()
    {
        LoggerConfiguration loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console();

        _ = Enum.TryParse(Environment.GetEnvironmentVariable("LOGLEVEL"), true, out LogEventLevel logLevel)
            ? loggerConfig.MinimumLevel.Is(logLevel)
            : loggerConfig.MinimumLevel.Information();

        _ = loggerConfig.Destructure.ByTransforming<LogEvent>(logEvent =>
        {
            return logEvent.Properties.TryGetValue("token", out LogEventPropertyValue? tokenValue) && tokenValue is ScalarValue token
                ? new LogEventProperty("token", new ScalarValue(token.Value!.ToString()!.MaskToken()))
                : logEvent;
        });

        Log.Logger = loggerConfig.CreateLogger();

        return Log.Logger;
    }
}
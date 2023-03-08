using BaseBotService.Extensions;
using Serilog;
using Serilog.Events;

namespace BaseBotService.Factories;

public static class LoggerFactory
{

    public static ILogger CreateLogger()
    {
        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console();

        if (Enum.TryParse<LogEventLevel>(Environment.GetEnvironmentVariable("LOGLEVEL"), true, out var logLevel))
        {
            loggerConfig.MinimumLevel.Is(logLevel);
        }
        else
        {
            loggerConfig.MinimumLevel.Information();
        }

        loggerConfig.Destructure.ByTransforming<LogEvent>(logEvent =>
        {
            if (logEvent.Properties.TryGetValue("token", out var tokenValue) && tokenValue is ScalarValue token)
            {
                return new LogEventProperty("token", new ScalarValue(token.Value.ToString()!.MaskToken()));
            }
            return logEvent;
        });

        Log.Logger = loggerConfig.CreateLogger();

        return Log.Logger;
    }
}
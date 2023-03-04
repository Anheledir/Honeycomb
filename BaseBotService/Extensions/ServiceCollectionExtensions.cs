using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace BaseBotService.Extensions;

internal static class ServiceCollectionExtensions
{

    public static IServiceCollection AddSerilogServices(this IServiceCollection services)
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
        return services.AddSingleton(Log.Logger);
    }
}
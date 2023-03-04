using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BaseBotService.Extensions;

internal static class ServiceCollectionExtensions
{

    public static IServiceCollection AddSerilogServices(this IServiceCollection services, LoggerConfiguration configuration)
    {
        switch (Environment.GetEnvironmentVariable("LOGLEVEL"))
        {
            case "information":
            case "info":
                configuration.MinimumLevel.Information();
                break;
            case "debug":
                configuration.MinimumLevel.Debug();
                break;
            case "verbose":
                configuration.MinimumLevel.Verbose();
                break;
            case "warning":
                configuration.MinimumLevel.Warning();
                break;
            case "error":
                configuration.MinimumLevel.Error();
                break;
            case "fatal":
                configuration.MinimumLevel.Fatal();
                break;
            default:
                configuration.MinimumLevel.Information();
                break;
        }

        Log.Logger = configuration.CreateLogger();
        return services.AddSingleton(Log.Logger);
    }
}
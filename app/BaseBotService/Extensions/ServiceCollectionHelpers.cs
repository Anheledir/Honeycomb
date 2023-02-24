using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BaseBotService.Extensions;

internal static class ServiceCollectionHelpers
{

    public static IServiceCollection AddSerilogServices(this IServiceCollection services, LoggerConfiguration configuration)
    {
        Log.Logger = configuration.CreateLogger();
        return services.AddSingleton(Log.Logger);
    }
}
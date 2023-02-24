using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BaseBotService.Extensions;

internal static class ServiceCollectionExtensions
{

    public static IServiceCollection AddSerilogServices(this IServiceCollection services, LoggerConfiguration configuration)
    {
        Log.Logger = configuration.CreateLogger();
        return services.AddSingleton(Log.Logger);
    }
}
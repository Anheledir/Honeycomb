using BaseBotService.Core;
using BaseBotService.Core.Interfaces;
using BaseBotService.Infrastructure;
using BaseBotService.Infrastructure.Services;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BaseBotService;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();

        IServiceProvider serviceProvider = host.Services;

        try
        {
            // Start Discord event listener
            DiscordEventListener listener = serviceProvider.GetRequiredService<DiscordEventListener>();
            await listener.StartAsync();

            // Connect to Discord
            DiscordSocketClient client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            IEnvironmentService environment = serviceProvider.GetRequiredService<IEnvironmentService>();

            if (!string.IsNullOrWhiteSpace(environment.DiscordBotToken))
            {
                await client.LoginAsync(TokenType.Bot, environment.DiscordBotToken, true);
                await client.StartAsync();
            }

            // Run the host
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger>();
            logger.Error(ex, "An unhandled exception occurred during the application execution.");
            throw;
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                // Register all services, including hosted services
                services.AddSingleton(LoggerFactory.CreateLogger())
                        .AddHostedService<HealthCheckService>()
                        .AddSingleton(ServiceFactory.CreateServiceProvider());
            });
}

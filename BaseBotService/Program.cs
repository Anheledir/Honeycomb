using BaseBotService.Core;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data;
using BaseBotService.Infrastructure;
using BaseBotService.Infrastructure.Services;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BaseBotService;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();

        IServiceProvider serviceProvider = host.Services;

        using (var scope = host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<HoneycombDbContext>();
            await dbContext.Database.MigrateAsync();
        }

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
            .ConfigureServices((context, services) =>
            {
                // Register all services from ServiceFactory
                ServiceFactory.ConfigureServices(services);

                // Add additional services, including hosted services
                services.AddHostedService<HealthCheckService>();
            });
}

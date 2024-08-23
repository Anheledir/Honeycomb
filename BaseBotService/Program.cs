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
    public static IServiceProvider ServiceProvider { get; internal set; } = ServiceFactory.CreateServiceProvider();

    private static void Main() => RunAsync().GetAwaiter().GetResult();

    private static async Task RunAsync()
    {
        DiscordEventListener listener = ServiceProvider.GetRequiredService<DiscordEventListener>();
        await listener.StartAsync();

        // Connect to Discord API
        DiscordSocketClient client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
        IEnvironmentService environment = ServiceProvider.GetRequiredService<IEnvironmentService>();

        if (!string.IsNullOrWhiteSpace(environment.DiscordBotToken))
        {
            await client.LoginAsync(TokenType.Bot, environment.DiscordBotToken, true);
            await client.StartAsync();
        }

        // Host the health check service
        IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices(services => services.AddHostedService<HealthCheckService>())
                .Build();

        await host.RunAsync(ServiceProvider.GetRequiredService<CancellationTokenSource>().Token);
    }
}
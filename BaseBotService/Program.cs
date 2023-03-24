using BaseBotService.Core;
using BaseBotService.Core.Interfaces;
using BaseBotService.Infrastructure;
using BaseBotService.Infrastructure.Services;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BaseBotService;

public class Program
{
    public static IServiceProvider ServiceProvider { get; } = ServiceFactory.CreateServiceProvider();

    private static void Main(string[] args) => new Program().RunAsync().GetAwaiter().GetResult();

    private async Task RunAsync()
    {
        DiscordEventListener listener = ServiceProvider.GetRequiredService<DiscordEventListener>();
        await listener.StartAsync();

        // Connect to Discord API
        DiscordSocketClient client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
        IEnvironmentService environment = ServiceProvider.GetRequiredService<IEnvironmentService>();

        await client.LoginAsync(TokenType.Bot, environment.DiscordBotToken);
        await client.StartAsync();

        // Host the health check service
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddHostedService<HealthCheckService>())
            .Build();

        await host.RunAsync();
    }
}
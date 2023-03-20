using BaseBotService.Services;
using BaseBotService.Utilities;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Honeycomb;

public class Program
{
    public static IServiceProvider ServiceProvider { get; } = ServiceFactory.CreateServiceProvider();

    static void Main(string[] args) => new Program().RunAsync().GetAwaiter().GetResult();

    async Task RunAsync()
    {
        var listener = ServiceProvider.GetRequiredService<DiscordEventListener>();
        await listener.StartAsync();

        // Connect to Discord API
        var client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
        var environment = ServiceProvider.GetRequiredService<IEnvironmentService>();

        await client.LoginAsync(TokenType.Bot, environment.DiscordBotToken);
        await client.StartAsync();

        // Host the health check service
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddHostedService<HealthCheckService>())
            .Build();

        await host.RunAsync();
    }
}
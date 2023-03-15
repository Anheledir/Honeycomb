using BaseBotService.Events;
using BaseBotService.Interfaces;
using BaseBotService.Services;
using BaseBotService.Utilities;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Honeycomb;

public class Program
{
    public static IServiceProvider ServiceProvider { get; } = ServiceFactory.CreateServiceProvider();

    static void Main(string[] args) => new Program().RunAsync().GetAwaiter().GetResult();

    async Task RunAsync()
    {
        // Load instances from DI
        var _client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
        var _environment = ServiceProvider.GetRequiredService<IEnvironmentService>();
        var _discordEvents = ServiceProvider.GetRequiredService<DiscordEvents>();
        var _handler = ServiceProvider.GetRequiredService<InteractionService>();

        // Process when the client is ready, so we can register our commands.
        _client.Log += _discordEvents.LogAsync;
        _handler.Log += _discordEvents.LogAsync;
        _client.Ready += _discordEvents.ReadyAsync;
        _client.Disconnected += _discordEvents.DisconnectedAsync;
        _client.MessageReceived += _discordEvents.MessageReceived;

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);

        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += _discordEvents.HandleInteraction;

        // Connect to Discord API
        await _client.LoginAsync(TokenType.Bot, _environment?.DiscordBotToken);
        await _client.StartAsync();

        // Host the health check service
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddHostedService<HealthCheckService>())
            .Build();

        await host.RunAsync();
    }
}
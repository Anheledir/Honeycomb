using BaseBotService.Events;
using BaseBotService.Interfaces;
using BaseBotService.Managers;
using BaseBotService.Services;
using Discord;
using Discord.Commands;
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
        // Load instances from DI
        var _client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
        var _commandService = ServiceProvider.GetRequiredService<CommandService>();
        var _environment = ServiceProvider.GetRequiredService<IEnvironmentService>();
        var _clientEvents = ServiceProvider.GetRequiredService<DiscordSocketClientEvents>();

        // Register logging events
        _client.Log += _clientEvents.LogAsync;
        _commandService.Log += _clientEvents.LogAsync;

        // Register event handlers
        _client.Ready += _clientEvents.ReadyAsync;
        _client.Disconnected += _clientEvents.DisconnectedAsync;
        _client.SlashCommandExecuted += _clientEvents.SlashCommandExecuted;
        _client.MessageReceived += _clientEvents.MessageReceived;

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
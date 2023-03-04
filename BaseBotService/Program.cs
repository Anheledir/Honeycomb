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
    public static IServiceProvider ServiceProvider { get; } = ServiceManager.RegisterServices();

    static void Main(string[] args) => new Program().RunAsync().GetAwaiter().GetResult();

    async Task RunAsync()
    {
        // Load instances from DI
        var _client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
        var _commandService = ServiceProvider.GetRequiredService<CommandService>();
        var _environment = ServiceProvider.GetRequiredService<IEnvironmentService>();
        var _events = ServiceProvider.GetRequiredService<DiscordSocketClientEvents>();

        // Register event handlers
        _client.Log += _events.LogAsync;
        _client.Ready += _events.ReadyAsync;
        _client.Disconnected += _events.DisconnectedAsync;
        _client.SlashCommandExecuted += _events.SlashCommandExecuted;
        _commandService.Log += _events.LogAsync;

        // Set log level from configuration


        // Connect to Discord API
        await _client.LoginAsync(TokenType.Bot, _environment?.DiscordBotToken);
        await _client.StartAsync();

        // Host the health check service
        IHost host = Host.CreateDefaultBuilder()
              .ConfigureServices(services =>
              {
                  services.AddHostedService<HealthCheckService>();
              })
            .Build();

        await host.RunAsync();
    }
}
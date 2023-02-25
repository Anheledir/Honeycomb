using BaseBotService.Events;
using BaseBotService.Helpers;
using BaseBotService.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Honeycomb;

public class Program
{

    public static IServiceProvider ServiceProvider { get; } = Services.RegisterServices();

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
        _client.SlashCommandExecuted += _events.SlashCommandExecuted;
        _commandService.Log += _events.LogAsync;

        // Connect to Discord API
        await _client.LoginAsync(TokenType.Bot, _environment?.DiscordBotToken);
        await _client.StartAsync();

        // Wait indefinitely for the bot to stay connected
        await Task.Delay(-1);
    }

}
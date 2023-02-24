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

    public static IServiceProvider ServiceProvider { get; } = ServiceHelpers.RegisterServices();

    static void Main(string[] args) => new Program().RunAsync().GetAwaiter().GetResult();

    async Task RunAsync()
    {
        var logger = ServiceProvider.GetService<ILogger>();

        // Load instances from DI
        var _client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
        var _commandService = ServiceProvider.GetRequiredService<CommandService>();
        var _commands = ServiceProvider.GetRequiredService<ICommandHandler>();
        var _environment = ServiceProvider.GetRequiredService<IEnvironmentHelper>();

        // Register event handlers
        _client.Log += DiscordSocketClientEvents.LogAsync;
        _client.Ready += DiscordSocketClientEvents.ReadyAsync;
        _client.SlashCommandExecuted += DiscordSocketClientEvents.SlashCommandExecuted;
        _commandService.Log += DiscordSocketClientEvents.LogAsync;

        // Install all command modules within the assembly
        await _commands.InstallCommandsAsync();

        // Connect to Discord API
        await _client.LoginAsync(TokenType.Bot, _environment?.DiscordBotToken);
        await _client.StartAsync();

        // Wait indefinitely for the bot to stay connected
        await Task.Delay(-1);
    }

}
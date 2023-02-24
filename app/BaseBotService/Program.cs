using BaseBotService.Base;
using BaseBotService.Helpers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Honeycomb;

public class Program
{
    private readonly IServiceProvider _serviceProvider;

    public Program()
    {
        _serviceProvider = ServiceHelpers.RegisterServices();

    }

    static void Main(string[] args) => new Program().RunAsync(args).GetAwaiter().GetResult();

    async Task RunAsync(string[] args)
    {
        ILogger _logger = _serviceProvider.GetService<ILogger>();

        // Create Discord bot client and initialize commands
        var _client = _serviceProvider.GetService<DiscordSocketClient>();
        var _commandService = _serviceProvider.GetService<CommandService>();
        var _commands = _serviceProvider.GetService<ICommandHandler>();

        // Register event handlers
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _commandService.Log += LogAsync;

        // Read environmental variables to initialize configuration
        string? token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
        if (token == null)
        {
            _logger.Fatal("Environment variable 'DISCORD_BOT_TOKEN' not set.");
            return;
        }

        // Install all command modules within the assembly
        await _commands.InstallCommandsAsync();

        // Connect to Discord API
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Wait indefinitely for the bot to stay connected
        await Task.Delay(-1);
    }

    /// <summary>
    /// Event-handler for the discord client to log messages with serilog.
    /// </summary>
    /// <param name="logMessage">The discord log-message.</param>
    private static Task LogAsync(LogMessage logMessage)
    {
        if (logMessage.Exception is CommandException cmdException)
        {
            Log.Write(LogEventLevel.Error, logMessage.Exception, $"[{cmdException.Source}] {cmdException.Context.User} failed to execute '{cmdException.Command.Name}' in {cmdException.Context.Channel}.");
            return Task.CompletedTask;
        }

        Log.Write(logMessage.GetSerilogSeverity(), logMessage.Exception, "[{Source}] {Message}", logMessage.Source, logMessage.Message);
        return Task.CompletedTask;
    }

    /// <summary>
    /// This method is executes when the bot finished startup, loaded all guilds and finished login.
    /// </summary>
    private async Task ReadyAsync()
    {
        Log.Information("Honeycomb is connected and ready.");
        await Task.CompletedTask;
    }
}

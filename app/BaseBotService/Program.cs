﻿using BaseBotService.Base;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;

namespace Honeycomb;

static class Program
{
    static async Task Main(string[] args)
    {
        // Configure Serilog logger
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        // Create Discord bot client and initialize commands
        var _client = new DiscordSocketClient();
        var _commandService = new CommandService();
        var commands = new CommandHandler(_client, _commandService);

        // Register event handlers
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _commandService.Log += LogAsync;

        // Read environmental variables to initialize configuration
        string? token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
        if (token == null)
        {
            Log.Fatal("Environment variable 'DISCORD_BOT_TOKEN' not set.");
            return;
        }

        // Install all command modules within the assembly
        await commands.InstallCommandsAsync();

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
            Log.Write(LogEventLevel.Error, logMessage.Exception, $"{cmdException.Context.User} failed to execute '{cmdException.Command.Name}' in {cmdException.Context.Channel}.");
            return Task.CompletedTask;
        }

        Log.Write(GetLogLevel(logMessage), logMessage.Exception, logMessage.Message);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Map a discord log-level to the corresponding serilog log-level.
    /// </summary>
    /// <param name="logMessage">The origin discord log-message.</param>
    /// <returns>The mapped serilog log-severity.</returns>
    private static LogEventLevel GetLogLevel(LogMessage logMessage)
    {
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
                return LogEventLevel.Fatal;
            case LogSeverity.Error:
                return LogEventLevel.Error;
            case LogSeverity.Warning:
                return LogEventLevel.Warning;
            case LogSeverity.Info:
                return LogEventLevel.Information;
            case LogSeverity.Verbose:
                return LogEventLevel.Debug;
            case LogSeverity.Debug:
                return LogEventLevel.Debug;
            default:
                return LogEventLevel.Verbose;
        }
    }

    /// <summary>
    /// This method is executes when the bot finished startup, loaded all guilds and finished login.
    /// </summary>
    private static async Task ReadyAsync()
    {
        Log.Information("Honeycomb is connected and ready.");
        await Task.CompletedTask;
    }
}

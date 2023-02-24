using BaseBotService.Extensions;
using BaseBotService.Helpers;
using BaseBotService.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Honeycomb;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

namespace BaseBotService.Events;

internal static class DiscordSocketClientEvents
{
    private static readonly ILogger _logger = Program.ServiceProvider.GetRequiredService<ILogger>();

    /// <summary>
    /// Event-handler for the discord client to log messages with serilog.
    /// </summary>
    /// <param name="logMessage">The discord log-message.</param>
    internal static Task LogAsync(LogMessage logMessage)
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
    internal static async Task ReadyAsync()
    {
        _logger.Information("Honeycomb is connected and ready.");

        var _client = Program.ServiceProvider.GetRequiredService<DiscordSocketClient>();
        var cmdHelper = new CommandHelpers(_logger, _client);

        await cmdHelper.RegisterGlobalCommandsAsync(false);
    }

    internal static async Task SlashCommandExecuted(SocketSlashCommand cmd)
    {
        var json = JsonConvert.SerializeObject(cmd, Formatting.Indented);
        _logger.Debug(json);

        switch (cmd.Data.Name)
        {
            // info module
            case "info":
                await Program.ServiceProvider.GetRequiredService<InfoModule>()?.InfoCommandAsync();
                break;

            // users module
            case "user-info":
                await Program.ServiceProvider.GetRequiredService<UsersModule>()?.UserinfoCommandAsync(cmd);
                break;
            case "user-roles":
                await Program.ServiceProvider.GetRequiredService<UsersModule>()?.ListRoleCommandAsync(cmd);
                break;

            // unknown
            default:
                await cmd.RespondAsync($"Unknown command {cmd.Data.Name}", ephemeral: true);
                break;
        }
    }

}
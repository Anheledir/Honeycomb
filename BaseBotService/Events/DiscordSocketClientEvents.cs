using BaseBotService.Enumeration;
using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using BaseBotService.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;

namespace BaseBotService.Events;

internal class DiscordSocketClientEvents
{
    private readonly ILogger _logger;
    private readonly IAssemblyService _assemblyService;
    private readonly IEnvironmentService _environmentHelper;
    private readonly ICommandManager _commandHelpers;
    private readonly IActivityPointsService _activityPoints;
    private readonly InfoModule _infoModule;
    private readonly UsersModule _usersModule;

    public DiscordSocketClientEvents(ILogger logger, IAssemblyService assemblyService, IEnvironmentService environmentHelper, ICommandManager commandHelpers, IActivityPointsService activityPoints, InfoModule infoModule, UsersModule usersModule)
    {
        _logger = logger;
        _assemblyService = assemblyService;
        _environmentHelper = environmentHelper;
        _commandHelpers = commandHelpers;
        _activityPoints = activityPoints;
        _infoModule = infoModule;
        _usersModule = usersModule;
    }

    internal Task DisconnectedAsync(Exception ex)
    {
        _logger.Warning(ex, "Lost connection to Discord.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Event-handler for the discord client to log messages with serilog.
    /// </summary>
    /// <param name="logMessage">The discord log-message.</param>
    internal Task LogAsync(LogMessage logMessage)
    {
        if (logMessage.Exception is CommandException cmdException)
        {
            _logger.Write(LogEventLevel.Error, logMessage.Exception, $"[{cmdException.Source}] {cmdException.Context.User} failed to execute '{cmdException.Command.Name}' in {cmdException.Context.Channel}.");
            return Task.CompletedTask;
        }

        _logger.Write(logMessage.GetSerilogSeverity(), logMessage.Exception, "[{Source}] {Message}", logMessage.Source, logMessage.Message);
        return Task.CompletedTask;
    }

    internal Task MessageReceived(SocketMessage arg)
    {
        IGuildUser? usr = arg.Author as IGuildUser;
        if (usr?.IsBot == true && usr.IsWebhook && (usr?.IsPending ?? false))
        {
            return _activityPoints.AddActivityTick(usr);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// This method is executes when the bot finished startup, loaded all guilds and finished login.
    /// </summary>
    internal async Task ReadyAsync()
    {
        _logger.Information($"{_assemblyService.Name} v{_assemblyService.Version} ({_environmentHelper.EnvironmentName}) is connected and ready.");

        switch (_environmentHelper.RegisterCommands)
        {
            case RegisterCommandsOnStartup.NoRegistration:
                _logger.Information("Skipping global application command registration.");
                break;
            case RegisterCommandsOnStartup.YesWithoutOverwrite:
                await _commandHelpers.RegisterGlobalCommandsAsync(false);
                break;
            case RegisterCommandsOnStartup.YesWithOverwrite:
                await _commandHelpers.RegisterGlobalCommandsAsync(true);
                break;
        }

    }

    internal async Task SlashCommandExecuted(SocketSlashCommand cmd)
    {
        switch (cmd.Data.Name)
        {
            // info module
            case "info":
                await _infoModule.InfoCommandAsync(cmd);
                _logger.Debug("Slash Command Executed: {@cmd}", cmd);
                break;

            // users module
            case "user-info":
                await _usersModule.UserinfoCommandAsync(cmd);
                _logger.Debug("Slash Command Executed: {@cmd}", cmd);
                break;
            case "user-roles":
                await _usersModule.ListRoleCommandAsync(cmd);
                _logger.Debug("Slash Command Executed: {@cmd}", cmd);
                break;

            // unknown
            default:
                await cmd.RespondAsync($"Unknown command {cmd.Data.Name}", ephemeral: true);
                _logger.Warning("Unknown command {@cmd}", cmd);
                break;
        }
    }

}
using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;

namespace BaseBotService.Events;

public class DiscordEvents
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;
    private readonly IAssemblyService _assemblyService;
    private readonly IEnvironmentService _environmentService;
    private readonly IEngagementService _activityPoints;
    private readonly InteractionService _handler;

    public DiscordEvents(ILogger logger, DiscordSocketClient client, IServiceProvider services, IAssemblyService assemblyService, IEnvironmentService environmentService, IEngagementService activityPoints, InteractionService handler)
    {
        _logger = logger;
        _client = client;
        _services = services;
        _assemblyService = assemblyService;
        _environmentService = environmentService;
        _activityPoints = activityPoints;
        _handler = handler;
    }

    public Task DisconnectedAsync(Exception ex)
    {
        _logger.Warning(ex, "Lost connection to Discord.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Event-handler for the discord client to log messages with serilog.
    /// </summary>
    /// <param name="logMessage">The discord log-message.</param>
    public Task LogAsync(LogMessage logMessage)
    {
        _logger.Write(logMessage.GetSerilogSeverity(), logMessage.Exception, "[{Source}] {Message}", logMessage.Source, logMessage.Message);
        return Task.CompletedTask;
    }

    public Task MessageReceived(IMessage message)
    {
        if (message.Channel is IDMChannel)
        {
            if (!message.Author.IsBot)
            {
                _logger.Debug($"DM from [{message.Author.Username}#{message.Author.Discriminator}]: {message.CleanContent}");
            }
            return Task.CompletedTask;
        }

        if (message.Author.IsBot || message.Author.IsWebhook)
        {
            _logger.Debug($"Message from [{message.Author.Username}#{message.Author.Discriminator}]: isBot = {message.Author.IsBot}, isWebhook = {message.Author.IsWebhook}");
            return Task.CompletedTask;
        }

        ulong userId = message.Author.Id;
        ulong guildId = message.Channel is IGuildChannel guildChannel ? guildChannel.GuildId : 0;
        _logger.Debug($"Guild ID: {guildId}, User ID: {userId}");
        if (guildId > 0)
        {
            _ = _activityPoints.AddActivityTick(guildId, userId);
        }
        return Task.CompletedTask;
    }


    /// <summary>
    /// This method is executes when the bot finished startup, loaded all guilds and finished login.
    /// </summary>
    public async Task ReadyAsync()
    {
        _logger.Information($"{_assemblyService.Name} v{_assemblyService.Version} ({_environmentService.EnvironmentName}) is connected and ready.");

        // TODO: Write a custom activity provider.
        await _client.SetActivityAsync(new Game("programming tutorials", ActivityType.Watching));
        await _client.SetStatusAsync(UserStatus.Online);

        IReadOnlyList<ModuleInfo> mod = _handler.Modules;
        if (mod != null)
        {
            foreach (var module in mod)
            {
                _logger.Information($"Registered module: {module.Name}");
            }
        }

        switch (_environmentService.RegisterCommands)
        {
            case Enumeration.RegisterCommandsOnStartup.NoRegistration:
                _logger.Information("Skipping global application command registration.");
                break;
            case Enumeration.RegisterCommandsOnStartup.YesWithoutOverwrite:
                _logger.Information("Registering global application commands.");
                await _handler.RegisterCommandsGloballyAsync();
                break;
            case Enumeration.RegisterCommandsOnStartup.YesWithOverwrite:
                _logger.Information("Registering global application commands and delete missing ones.");
                await _handler.RegisterCommandsGloballyAsync(true);
                break;
        }
    }

    public async Task HandleInteraction(IDiscordInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, (SocketInteraction)interaction);

            // Execute the incoming command.
            var result = await _handler.ExecuteCommandAsync(context, _services);


            if (!result.IsSuccess)
            {
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        _logger.Information("Unmet Precondition: {0}", result.ErrorReason);
                        break;
                    case InteractionCommandError.UnknownCommand:
                        _logger.Warning("Unknown command: {0}", result.Error);
                        break;
                    case InteractionCommandError.BadArgs:
                        _logger.Information("Invalid number or arguments: {0}", result.Error);
                        break;
                    case InteractionCommandError.Exception:
                        _logger.Warning("Command exception: {0}", result.ErrorReason);
                        break;
                    case InteractionCommandError.Unsuccessful:
                        _logger.Warning("Command could not be executed: {0}", result.ErrorReason);
                        break;
                    default:
                        _logger.Error("Unknown error: {0}", result.Error);
                        break;
                }

                var errorEmbed = new EmbedBuilder()
                        .WithTitle("Error!")
                        .WithDescription($"**Error Message:** {result.ErrorReason}")
                        .WithAuthor(context.User)
                        .WithColor(Color.Red).Build();

                if (context.Interaction.HasResponded)
                    await context.Interaction.FollowupAsync(embed: errorEmbed);
                else
                    await context.Interaction.RespondAsync(embed: errorEmbed);
            }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgment will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
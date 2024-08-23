using BaseBotService.Core.Messages;
using Discord.WebSocket;

namespace BaseBotService.Interactions;

/// <summary>
/// Handles incoming interactions and executes the corresponding commands.
/// </summary>
public class InteractionHandler : INotificationHandler<InteractionCreatedNotification>
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;
    private readonly InteractionService _interaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionHandler"/> class.
    /// </summary>
    public InteractionHandler(ILogger logger, DiscordSocketClient client, IServiceProvider services, InteractionService interaction)
    {
        _logger = logger.ForContext<InteractionHandler>();
        _client = client;
        _services = services;
        _interaction = interaction;
    }

    /// <summary>
    /// Handles the interaction created notification by executing the corresponding command.
    /// </summary>
    public async Task Handle(InteractionCreatedNotification arg, CancellationToken cancellationToken)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, arg.Interaction);

            _logger.Debug("Interaction {type} by {UserId} in {ChannelId} ({GuildId}) for '{@cmdData}'",
                context.Interaction.Type,
                context.User.Id,
                context.Channel.Id,
                context.Guild?.Id,
                context.Interaction.Data);

            // Execute the incoming command.
            var result = await _interaction.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
            {
                HandleCommandError(context, result);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while handling the interaction.");

            // If the Slash Command execution fails, it's likely the original interaction acknowledgment
            // will persist. It's a good idea to delete the original response, or at least let the user know
            // something went wrong during the command execution.
            if (arg.Interaction.Type is InteractionType.ApplicationCommand)
            {
                try
                {
                    var response = await arg.Interaction.GetOriginalResponseAsync();
                    await response.DeleteAsync();
                }
                catch (Exception deleteEx)
                {
                    _logger.Error(deleteEx, "Failed to delete the original interaction response.");
                }
            }
        }
    }

    /// <summary>
    /// Handles command errors by logging them and responding to the user with an error message.
    /// </summary>
    private async void HandleCommandError(SocketInteractionContext context, IResult result)
    {
        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                _logger.Information("Unmet Precondition: {0}", result.ErrorReason);
                break;
            case InteractionCommandError.UnknownCommand:
                _logger.Warning("Unknown command: {0}", result.ErrorReason);
                break;
            case InteractionCommandError.BadArgs:
                _logger.Information("Invalid number or arguments: {0}", result.ErrorReason);
                break;
            case InteractionCommandError.Exception:
                _logger.Warning("Command exception: {0}", result.ErrorReason);
                break;
            case InteractionCommandError.Unsuccessful:
                _logger.Warning("Command could not be executed: {0}", result.ErrorReason);
                break;
            default:
                _logger.Error("Unknown error: {0}", result.ErrorReason);
                break;
        }

        var errorEmbed = new EmbedBuilder()
            .WithTitle("Error!")
            .WithDescription($"**Error Message:** {result.ErrorReason}")
            .WithAuthor(context.User)
            .WithColor(Color.Red)
            .Build();

        if (context.Interaction.HasResponded)
        {
            await context.Interaction.FollowupAsync(embed: errorEmbed);
        }
        else
        {
            await context.Interaction.RespondAsync(embed: errorEmbed);
        }
    }
}

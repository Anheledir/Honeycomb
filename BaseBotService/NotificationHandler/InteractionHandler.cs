using BaseBotService.Notifications;
using Discord.WebSocket;

namespace BaseBotService.NotificationHandler;
public class InteractionHandler : INotificationHandler<InteractionCreatedNotification>
{
    private readonly ILogger _logger;
    private readonly InteractionService _interaction;
    private readonly IServiceProvider _services;
    private readonly DiscordSocketClient _client;

    public InteractionHandler(ILogger logger, InteractionService interaction, IServiceProvider services, DiscordSocketClient client)
    {
        _logger = logger;
        _interaction = interaction;
        _services = services;
        _client = client;
    }

    public async Task Handle(InteractionCreatedNotification arg, CancellationToken cancellationToken)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, arg.Interaction);

            // Execute the incoming command.
            var result = await _interaction.ExecuteCommandAsync(context, _services);

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
            if (arg.Interaction.Type is InteractionType.ApplicationCommand)
            {
                await arg.Interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}

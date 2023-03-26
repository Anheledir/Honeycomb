using BaseBotService.Core.Messages;
using Discord.WebSocket;
using Serilog.Formatting.Json;

namespace BaseBotService.Interactions;
public class InteractionHandler : INotificationHandler<InteractionCreatedNotification>
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;
    private readonly InteractionService _interaction;

    public InteractionHandler(ILogger logger, DiscordSocketClient client, IServiceProvider services, InteractionService interaction)
    {
        _logger = logger.ForContext<InteractionHandler>();
        _client = client;
        _services = services;
        _interaction = interaction;
    }

    public async Task Handle(InteractionCreatedNotification arg, CancellationToken cancellationToken)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of InteractionModuleBase<T> modules.
            SocketInteractionContext context = new(_client, arg.Interaction);

            _logger.Debug("Interaction {type} by {UserId} in {ChannelId} ({GuildId}) for '{@cmdData}'",
               context.Interaction.Type,
               context.User.Id,
               context.Channel.Id,
               context.Guild?.Id,
               context.Interaction.Data);

            // Execute the incoming command.
            IResult result = await _interaction.ExecuteCommandAsync(context, _services);

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

                Embed errorEmbed = new EmbedBuilder()
                        .WithTitle("Error!")
                        .WithDescription($"**Error Message:** {result.ErrorReason}")
                        .WithAuthor(context.User)
                        .WithColor(Color.Red).Build();

                if (context.Interaction.HasResponded)
                {
                    _ = await context.Interaction.FollowupAsync(embed: errorEmbed);
                }
                else
                {
                    await context.Interaction.RespondAsync(embed: errorEmbed);
                }
            }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgment
            // will persist. It is a good idea to delete the original response, or at least let the user know
            // that something went wrong during the command execution.
            if (arg.Interaction.Type is InteractionType.ApplicationCommand)
            {
                _ = await arg.Interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}

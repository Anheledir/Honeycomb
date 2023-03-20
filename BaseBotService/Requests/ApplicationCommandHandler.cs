using BaseBotService.Messages;

namespace BaseBotService.Requests;
internal class ApplicationCommandHandler : IRequestHandler<ApplicationCommandRequest>
{
    private readonly ILogger _logger;
    private readonly InteractionService _interaction;
    private readonly IServiceProvider _services;

    public ApplicationCommandHandler(ILogger logger, InteractionService interaction, IServiceProvider services)
    {
        _logger = logger;
        _interaction = interaction;
        _services = services;
    }

    public async Task Handle(ApplicationCommandRequest msg, CancellationToken cancellationToken)
    {
        _logger.Debug("Application Command by {UserId} in {ChannelId} ({GuildId}) for '{@cmdData}'",
            msg.Context.User.Id,
            msg.Context.Channel.Id,
            msg.Context.Guild?.Id,
            msg.Context.Interaction.Data);

        // Execute the incoming command.
        var result = await _interaction.ExecuteCommandAsync(msg.Context, _services);

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
                    .WithAuthor(msg.Context.User)
                    .WithColor(Color.Red).Build();

            if (msg.Context.Interaction.HasResponded)
                await msg.Context.Interaction.FollowupAsync(embed: errorEmbed);
            else
                await msg.Context.Interaction.RespondAsync(embed: errorEmbed);
        }

    }
}
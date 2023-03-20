using BaseBotService.Messages;

namespace BaseBotService.Requests;
public class AutocompleteCommandHandler : IRequestHandler<AutocompleteCommandRequest>
{
    private readonly ILogger _logger;

    public AutocompleteCommandHandler(ILogger logger)
    {
        _logger = logger;
    }

    async Task IRequestHandler<AutocompleteCommandRequest>.Handle(AutocompleteCommandRequest msg, CancellationToken cancellationToken)
    {
        _logger.Debug("Autocomplete Command by {UserId} in {ChannelId} ({GuildId}) for '{@cmdData}'",
            msg.Context.User.Id,
            msg.Context.Channel.Id,
            msg.Context.Guild?.Id,
            msg.Context.Interaction.Data);


        if (msg.Context.Interaction.HasResponded)
            await msg.Context.Interaction.FollowupAsync("Followup Done.");
        else
            await msg.Context.Interaction.RespondAsync("Response Done");
    }
}

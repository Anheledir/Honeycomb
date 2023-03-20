using BaseBotService.Messages;

namespace BaseBotService.Requests;
public class MessageComponentHandler : IRequestHandler<MessageComponentRequest>
{
    private readonly ILogger _logger;
    private readonly InteractionService _interaction;
    private readonly IServiceProvider _services;

    public MessageComponentHandler(ILogger logger, InteractionService interaction, IServiceProvider services)
    {
        _logger = logger;
        _interaction = interaction;
        _services = services;
    }

    async Task IRequestHandler<MessageComponentRequest>.Handle(MessageComponentRequest msg, CancellationToken cancellationToken)
    {
        _logger.Debug("Message Component by {UserId} in {ChannelId} ({GuildId}) for '{@cmdData}'",
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

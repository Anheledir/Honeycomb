using BaseBotService.Core.Messages;
using Discord.WebSocket;

namespace BaseBotService.Interactions;
public class MessageComponentHandler : IRequestHandler<MessageComponentRequest>
{
    private readonly ILogger _logger;

    public MessageComponentHandler(ILogger logger)
    {
        _logger = logger;
    }

    async Task IRequestHandler<MessageComponentRequest>.Handle(MessageComponentRequest msg, CancellationToken cancellationToken)
    {
        SocketMessageComponent? component = msg.Context.Interaction as SocketMessageComponent;

        _logger.Debug("Message Component by {UserId} in {ChannelId} ({GuildId}) for {@cmdData}",
            msg.Context.User.Id,
            msg.Context.Channel.Id,
            msg.Context.Guild?.Id,
            component!.Data);
        string message = component!.Data.Type switch
        {
            ComponentType.ActionRow => $"ActionRow {component.Data.CustomId}",
            ComponentType.Button => $"Button {component.Data.CustomId}",
            ComponentType.SelectMenu => $"Select {component.Data.CustomId}: {string.Join(',', component.Data.Values)}",
            ComponentType.TextInput => $"Input {component.Data.CustomId}: {component.Data.Value}",
            ComponentType.ModalSubmit => $"ModalSubmit {component.Data.CustomId}",
            _ => $"Unknown type {component.Data.Type}!",
        };
        if (msg.Context.Interaction.HasResponded)
        {
            _ = await msg.Context.Interaction.FollowupAsync(message);
        }
        else
        {
            await msg.Context.Interaction.RespondAsync(message);
        }
    }
}

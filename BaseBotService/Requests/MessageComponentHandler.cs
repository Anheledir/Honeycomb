using BaseBotService.Messages;
using Discord.WebSocket;

namespace BaseBotService.Requests;
public class MessageComponentHandler : IRequestHandler<MessageComponentRequest>
{
    private readonly ILogger _logger;

    public MessageComponentHandler(ILogger logger)
    {
        _logger = logger;
    }

    async Task IRequestHandler<MessageComponentRequest>.Handle(MessageComponentRequest msg, CancellationToken cancellationToken)
    {
        var component = msg.Context.Interaction as SocketMessageComponent;

        _logger.Debug("Message Component by {UserId} in {ChannelId} ({GuildId}) for {@cmdData}",
            msg.Context.User.Id,
            msg.Context.Channel.Id,
            msg.Context.Guild?.Id,
            component.Data);

        string message = string.Empty;
        switch (component.Data.Type)
        {
            case ComponentType.ActionRow:
                message = $"ActionRow {component.Data.CustomId}";
                break;
            case ComponentType.Button:
                message = $"Button {component.Data.CustomId}";
                break;
            case ComponentType.SelectMenu:
                message = $"Select {component.Data.CustomId}: {string.Join(',', component.Data.Values)}";
                break;
            case ComponentType.TextInput:
                message = $"Input {component.Data.CustomId}: {component.Data.Value}";
                break;
            case ComponentType.ModalSubmit:
                message = $"ModalSubmit {component.Data.CustomId}";
                break;
            default:
                message = $"Unknown type {component.Data.Type}!";
                break;
        }

        if (msg.Context.Interaction.HasResponded)
            await msg.Context.Interaction.FollowupAsync(message);
        else
            await msg.Context.Interaction.RespondAsync(message);
    }
}

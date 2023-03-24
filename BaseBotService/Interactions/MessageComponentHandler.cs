using BaseBotService.Core.Interfaces;
using BaseBotService.Core.Messages;
using Discord.WebSocket;

namespace BaseBotService.Interactions;
internal class MessageComponentHandler : IRequestHandler<MessageComponentRequest>
{
    private readonly ILogger _logger;
    private readonly IComponentStrategyFactory _strategyFactory;

    public MessageComponentHandler(ILogger logger, IComponentStrategyFactory strategyFactory)
    {
        _logger = logger.ForContext<MessageComponentHandler>();
        _strategyFactory = strategyFactory;
    }

    public async Task Handle(MessageComponentRequest msg, CancellationToken cancellationToken)
    {
        SocketMessageComponent? component = msg.Context.Interaction as SocketMessageComponent;

        _logger.Debug("Message Component by {UserId} in {ChannelId} ({GuildId}) for {@cmdData}",
            msg.Context.User.Id,
            msg.Context.Channel.Id,
            msg.Context.Guild?.Id,
            component!.Data);

        IComponentStrategy? strategy = _strategyFactory.GetStrategy(component.Data.Type);
        if (strategy != null)
        {
            await strategy.ExecuteAsync(component.Data.CustomId, msg.Context);
        }
        else
        {
            _logger.Error("No strategy implemented for component type {type}.", component.Data.Type);
        }

        if (!msg.Context.Interaction.HasResponded)
        {
            await msg.Context.Interaction.RespondAsync("Done.", ephemeral: true);
        }
    }
}

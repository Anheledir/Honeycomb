using BaseBotService.Core;

namespace BaseBotService.Infrastructure.Interactions;

public class ClientDisconnectedHandler : INotificationHandler<DiscordEventListener.ClientDisconnectedNotification>
{
    private readonly ILogger _logger;

    public ClientDisconnectedHandler(ILogger logger)
    {
        _logger = logger.ForContext<ClientDisconnectedHandler>();
    }

    public Task Handle(DiscordEventListener.ClientDisconnectedNotification arg, CancellationToken cancellationToken)
    {
        _logger.Warning(arg.DisconnectException, "Lost connection to Discord.");
        return Task.CompletedTask;
    }
}

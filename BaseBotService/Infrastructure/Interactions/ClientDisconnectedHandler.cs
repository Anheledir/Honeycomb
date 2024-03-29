﻿using BaseBotService.Core.Messages;

namespace BaseBotService.Infrastructure.Interactions;

public class ClientDisconnectedHandler : INotificationHandler<ClientDisconnectedNotification>
{
    private readonly ILogger _logger;

    public ClientDisconnectedHandler(ILogger logger)
    {
        _logger = logger.ForContext<ClientDisconnectedHandler>();
    }

    public Task Handle(ClientDisconnectedNotification arg, CancellationToken cancellationToken)
    {
        _logger.Warning(arg.DisconnectException, "Lost connection to Discord.");
        return Task.CompletedTask;
    }
}

using BaseBotService.Messages;

namespace BaseBotService.Notifications;

public class LogHandler : INotificationHandler<LogNotification>
{
    private readonly ILogger _logger;

    public LogHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(LogNotification arg, CancellationToken cancellationToken)
    {
        _logger.Write(
            level: arg.LogMessage.GetSerilogSeverity(),
            exception: arg.LogMessage.Exception,
            messageTemplate: "[{Source}] {Message}",
            propertyValue0: arg.LogMessage.Source,
            propertyValue1: arg.LogMessage.Message);
        return Task.CompletedTask;
    }
}

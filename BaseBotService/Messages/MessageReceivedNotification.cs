namespace BaseBotService.Messages;

public class MessageReceivedNotification : INotification
{
    public MessageReceivedNotification(IMessage message) => Message = message ?? throw new ArgumentNullException(nameof(message));

    public IMessage Message { get; }
}
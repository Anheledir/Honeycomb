namespace BaseBotService.Messages;

public class ClientDisconnectedNotification : INotification
{
    public ClientDisconnectedNotification(Exception ex)
    {
        DisconnectException = ex;
    }

    public Exception DisconnectException { get; set; }
}
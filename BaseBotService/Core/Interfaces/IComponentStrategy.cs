namespace BaseBotService.Core.Interfaces;

public interface IComponentStrategy
{
    Task ExecuteAsync(string customId, SocketInteractionContext context);
}
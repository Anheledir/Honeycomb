namespace BaseBotService.Core.Interfaces;
public interface IComponentStrategyFactory
{
    IComponentStrategy? GetStrategy(ComponentType type);
}
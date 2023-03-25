using BaseBotService.Core.Interfaces;
using BaseBotService.Infrastructure.Strategies;

namespace BaseBotService.Infrastructure;

public class ComponentStrategyFactory : IComponentStrategyFactory
{
    private readonly Dictionary<ComponentType, IComponentStrategy?> _strategies;
    private readonly ILogger _logger;

    public ComponentStrategyFactory(
            ILogger logger,
            ButtonStrategy buttonStrategy,
            ActionRowStrategy actionRowStrategy,
            ModalSubmitStrategy modalSubmitStrategy,
            TextInputStrategy textInputStrategy,
            SelectMenuStrategy selectMenuStrategy
        )
    {
        _strategies = new Dictionary<ComponentType, IComponentStrategy?>
        {
            {ComponentType.Button, buttonStrategy},
            {ComponentType.ActionRow, actionRowStrategy},
            {ComponentType.ModalSubmit, modalSubmitStrategy},
            {ComponentType.TextInput, textInputStrategy},
            {ComponentType.SelectMenu, selectMenuStrategy},
        };
        _logger = logger;
    }

    public IComponentStrategy? GetStrategy(ComponentType type)
    {
        if (_strategies.TryGetValue(type, out var strategy))
        {
            _logger.Debug($"Found strategy {strategy?.GetType()} for type {type}.");
            return strategy;
        }

        _logger.Error($"No strategy implemented for component type {type}.");
        return null;
    }
}
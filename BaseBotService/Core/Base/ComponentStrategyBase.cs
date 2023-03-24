using BaseBotService.Core.Interfaces;

namespace BaseBotService.Core.Base;

public class ComponentStrategyBase : IComponentStrategy
{
    private readonly ILogger _logger;

    public ComponentStrategyBase(ILogger logger, Dictionary<string, Func<SocketInteractionContext, Task>>? actions = null)
    {
        _logger = logger; // Uses the context of the implementing class.
        Actions = actions;
    }

    public Dictionary<string, Func<SocketInteractionContext, Task>>? Actions { get; protected set; }

    public async Task ExecuteAsync(string customId, SocketInteractionContext context)
    {
        if (Actions!.TryGetValue(customId, out var action))
        {
            _logger.Debug("Found {@action} for {customId}", action, customId);
            await action(context);
        }
        else
        {
            _logger.Error($"There was no command with custom-id '{customId}' being found.");
        }
    }
}
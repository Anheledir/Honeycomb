using BaseBotService.Commands;
using BaseBotService.Core.Base;

namespace BaseBotService.Infrastructure.Strategies;

public class ButtonStrategy : ComponentStrategyBase
{
    private readonly ILogger _logger;

    public ButtonStrategy(
        ILogger logger,
        UserModule userModule,
        Dictionary<string, Func<SocketInteractionContext, Task>>? actions = null)
        : base(logger, actions)
    {
        _logger = logger.ForContext<ButtonStrategy>();
        Actions = actions ?? new Dictionary<string, Func<SocketInteractionContext, Task>>
        {
            {"usr-profile-main", UserModule.GoBackProfileMain},
            {"usr-profile-close", UserModule.DeleteMessageDelayed},
        };

        _logger.Debug($"Initialized {nameof(ButtonStrategy)} with {Actions.Count} dispatchers.");
        if (actions != null)
        {
            _logger.Verbose("Injected dictionary {@actions}", actions);
        }
        else
        {
            _logger.Verbose("Used default dictionary.");
        }
    }
}
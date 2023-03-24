using BaseBotService.Core.Base;

namespace BaseBotService.Infrastructure.Strategies;

public class ButtonStrategy : ComponentStrategyBase
{
    private readonly ILogger _logger;

    public ButtonStrategy(ILogger logger, Dictionary<string, Func<SocketInteractionContext, Task>>? actions = null)
        : base(logger, actions)
    {
        _logger = logger.ForContext<ButtonStrategy>();
        Actions = actions ?? new Dictionary<string, Func<SocketInteractionContext, Task>>
        {
            //{"user-profile-cancel", UserProfileCancelAsync},
            //{"user-profile-save", UserProfileSaveAsync},
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
using BaseBotService.Commands;
using BaseBotService.Core.Base;

namespace BaseBotService.Infrastructure.Strategies;

public class SelectMenuStrategy : ComponentStrategyBase
{
    private readonly ILogger _logger;

    public SelectMenuStrategy(
        ILogger logger,
        UserModule userModule,
        Dictionary<string, Func<SocketInteractionContext, Task>>? actions = null)
        : base(logger, actions)
    {
        _logger = logger.ForContext<SelectMenuStrategy>();
        Actions = actions ?? new Dictionary<string, Func<SocketInteractionContext, Task>>
        {
            {"usr-profile-config", userModule.UserProfileCountry},
            {"usr-profile-country", userModule.SaveProfileCountry},
            {"usr-profile-languages", userModule.SaveProfileLanguages},
            {"usr-profile-gender", userModule.SaveProfileGenderIdentity},
            {"usr-profile-timezone", userModule.SaveProfileTimezone},
        };

        _logger.Debug($"Initialized {nameof(SelectMenuStrategy)} with {Actions.Count} dispatchers.");
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
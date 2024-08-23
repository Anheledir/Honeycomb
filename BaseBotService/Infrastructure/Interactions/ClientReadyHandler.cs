using BaseBotService.Core;
using BaseBotService.Core.Enums;
using BaseBotService.Core.Interfaces;

namespace BaseBotService.Infrastructure.Interactions;

public class ClientReadyHandler : INotificationHandler<DiscordEventListener.ClientReadyNotification>
{
    private readonly ILogger _logger;
    private readonly IEnvironmentService _environmentService;
    private readonly IAssemblyService _assemblyService;
    private readonly InteractionService _interactionService;
    private readonly ITranslationService _translationService;
    private readonly IMediator _mediator;

    public ClientReadyHandler(ILogger logger, IEnvironmentService environmentService, IAssemblyService assemblyService, InteractionService interactionService, ITranslationService translationService, IMediator mediator)
    {
        _logger = logger.ForContext<ClientReadyHandler>();
        _environmentService = environmentService;
        _assemblyService = assemblyService;
        _interactionService = interactionService;
        _translationService = translationService;
        _mediator = mediator;
    }

    public async Task Handle(DiscordEventListener.ClientReadyNotification notification, CancellationToken cancellationToken)
    {
        _logger.Information($"{_assemblyService.Name} v{_assemblyService.Version} ({_environmentService.EnvironmentName}) is connected and ready.");

        IReadOnlyList<ModuleInfo> mod = _interactionService.Modules;
        if (mod != null)
        {
            foreach (ModuleInfo module in mod)
            {
                _logger.Information($"Registered module: {module.Name}");
            }
        }

        switch (_environmentService.RegisterCommands)
        {
            case RegisterCommandsOnStartup.NoRegistration:
                _logger.Information("Skipping global application command registration.");
                break;
            case RegisterCommandsOnStartup.YesWithoutOverwrite:
                _logger.Information("Registering global application commands.");
                _ = await _interactionService.RegisterCommandsGloballyAsync();
                break;
            case RegisterCommandsOnStartup.YesWithOverwrite:
                _logger.Information("Registering global application commands and delete missing ones.");
                _ = await _interactionService.RegisterCommandsGloballyAsync(true);
                break;
        }

        await _mediator.Publish(new DiscordEventListener.UpdateActivityNotification(_translationService.GetString("default-activity"), UserStatus.DoNotDisturb), cancellationToken);
    }
}

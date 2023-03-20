using BaseBotService.Messages;

namespace BaseBotService.Notifications;

public class ClientReadyHandler : INotificationHandler<ClientReadyNotification>
{
    private readonly ILogger _logger;
    private readonly IEnvironmentService _environmentService;
    private readonly IAssemblyService _assemblyService;
    private readonly InteractionService _interactionService;

    public ClientReadyHandler(ILogger logger, IEnvironmentService environmentService, IAssemblyService assemblyService, InteractionService interactionService)
    {
        _logger = logger;
        _environmentService = environmentService;
        _assemblyService = assemblyService;
        _interactionService = interactionService;
    }

    public async Task Handle(ClientReadyNotification notification, CancellationToken cancellationToken)
    {
        _logger.Information($"{_assemblyService.Name} v{_assemblyService.Version} ({_environmentService.EnvironmentName}) is connected and ready.");

        IReadOnlyList<ModuleInfo> mod = _interactionService.Modules;
        if (mod != null)
        {
            foreach (var module in mod)
            {
                _logger.Information($"Registered module: {module.Name}");
            }
        }

        switch (_environmentService.RegisterCommands)
        {
            case Enumeration.RegisterCommandsOnStartup.NoRegistration:
                _logger.Information("Skipping global application command registration.");
                break;
            case Enumeration.RegisterCommandsOnStartup.YesWithoutOverwrite:
                _logger.Information("Registering global application commands.");
                await _interactionService.RegisterCommandsGloballyAsync();
                break;
            case Enumeration.RegisterCommandsOnStartup.YesWithOverwrite:
                _logger.Information("Registering global application commands and delete missing ones.");
                await _interactionService.RegisterCommandsGloballyAsync(true);
                break;
        }
    }
}

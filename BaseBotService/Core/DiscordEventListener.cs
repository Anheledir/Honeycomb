using BaseBotService.Core.Messages;
using Discord.WebSocket;
using System.Reflection;

namespace BaseBotService.Core;

public class DiscordEventListener
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;
    private readonly InteractionService _handler;
    private readonly IMediator _mediator;
    private readonly CancellationToken _cancellationToken;

    public DiscordEventListener(
        ILogger logger,
        DiscordSocketClient client,
        IServiceProvider services,
        InteractionService handler,
        IMediator mediator)
    {
        _logger = logger.ForContext<DiscordEventListener>();
        _client = client;
        _services = services;
        _handler = handler;
        _mediator = mediator;
        _cancellationToken = new CancellationTokenSource().Token;
    }

    public async Task StartAsync()
    {
        _logger.Information("Starting Discord event listener.");

        _client.MessageReceived += (socketMessage) => _mediator.Publish(new MessageReceivedNotification(socketMessage), _cancellationToken);
        _client.Log += (msg) => _mediator.Publish(new LogNotification(msg), _cancellationToken);
        _client.ReactionAdded += (cache, channel, reaction) => _mediator.Publish(new ReactionAddedNotification(cache, channel, reaction), _cancellationToken);
        _client.Ready += () => _mediator.Publish(new ClientReadyNotification(), _cancellationToken);
        _client.Disconnected += (ex) => _mediator.Publish(new ClientDisconnectedNotification(ex), _cancellationToken);
        _client.InteractionCreated += (interaction) => _mediator.Publish(new InteractionCreatedNotification(interaction), _cancellationToken);
        _handler.Log += (msg) => _mediator.Publish(new LogNotification(msg), _cancellationToken);

        _ = await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        _logger.Information($"Found {_handler.Modules.Count} modules.");

        await Task.CompletedTask;
    }
}
using BaseBotService.Commands.Interfaces;
using BaseBotService.Core.Messages;
using Discord.WebSocket;

namespace BaseBotService.Interactions;

/// <summary>
/// Handles message received notifications for engagement tracking and logging.
/// </summary>
public class EngagementServiceHandler : INotificationHandler<MessageReceivedNotification>
{
    private readonly ILogger _logger;
    private readonly IEngagementService _engagementService;
    private readonly DiscordSocketClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="EngagementServiceHandler"/> class.
    /// </summary>
    public EngagementServiceHandler(ILogger logger, IEngagementService engagementService, DiscordSocketClient client)
    {
        _logger = logger.ForContext<EngagementServiceHandler>();
        _engagementService = engagementService;
        _client = client;
    }

    /// <summary>
    /// Handles the MessageReceivedNotification event to log and track user engagement.
    /// </summary>
    public async Task Handle(MessageReceivedNotification arg, CancellationToken cancellationToken)
    {
        // Ignore messages sent by the bot itself
        if (IsSelfMessage(arg.Message))
        {
            return;
        }

        // Log direct messages (DMs) from users
        if (arg.Message.Channel is IDMChannel && !arg.Message.Author.IsBot)
        {
            _logger.Debug("DM from [{Username}#{Discriminator}]: {Message}", arg.Message.Author.Username, arg.Message.Author.Discriminator, arg.Message.CleanContent);
        }

        // Log messages from bots and webhooks
        if (arg.Message.Author.IsBot || arg.Message.Author.IsWebhook)
        {
            _logger.Debug("Message from [{Username}#{Discriminator}]: isBot = {IsBot}, isWebhook = {IsWebhook}",
                arg.Message.Author.Username, arg.Message.Author.Discriminator, arg.Message.Author.IsBot, arg.Message.Author.IsWebhook);
        }

        // Track user engagement for guild messages
        if (arg.Message.Channel is IGuildChannel guildChannel)
        {
            ulong userId = arg.Message.Author.Id;
            ulong guildId = guildChannel.GuildId;

            _logger.Debug("Message in guild {GuildId} by user {UserId}", guildId, userId);
            await _engagementService.AddActivityTickAsync(guildId, userId);
        }
    }

    /// <summary>
    /// Checks if the message was sent by the bot itself.
    /// </summary>
    private bool IsSelfMessage(IMessage message) => message.Author.Id == _client.CurrentUser.Id;
}

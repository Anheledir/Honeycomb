using BaseBotService.Messages;

namespace BaseBotService.Notifications;

public class MessageReceivedHandler : INotificationHandler<MessageReceivedNotification>
{
    private readonly ILogger _logger;
    private readonly IEngagementService _engagementService;

    public MessageReceivedHandler(ILogger logger, IEngagementService engagementService)
    {
        _logger = logger;
        _engagementService = engagementService;
    }

    public Task Handle(MessageReceivedNotification arg, CancellationToken cancellationToken)
    {
        if (arg.Message.Channel is IDMChannel && !arg.Message.Author.IsBot)
        {
            _logger.Debug($"DM from [{arg.Message.Author.Username}#{arg.Message.Author.Discriminator}]: {arg.Message.CleanContent}");
        }

        if (arg.Message.Author.IsBot || arg.Message.Author.IsWebhook)
        {
            _logger.Debug($"Message from [{arg.Message.Author.Username}#{arg.Message.Author.Discriminator}]: isBot = {arg.Message.Author.IsBot}, isWebhook = {arg.Message.Author.IsWebhook}");
        }

        ulong userId = arg.Message.Author.Id;
        ulong guildId = arg.Message.Channel is IGuildChannel guildChannel ? guildChannel.GuildId : 0;
        _logger.Debug($"Guild ID: {guildId}, User ID: {userId}");
        if (guildId > 0)
        {
            _ = _engagementService.AddActivityTick(guildId, userId);
        }

        return Task.CompletedTask;
    }
}
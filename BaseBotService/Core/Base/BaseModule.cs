using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Attributes;
using Discord.WebSocket;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BaseBotService.Core.Base;

/// <summary>
/// Base class for all interaction modules in the bot, providing common functionality such as logging, rate limiting, and response handling.
/// </summary>
public abstract class BaseModule : InteractionModuleBase<SocketInteractionContext>
{
    #region Properties

    /// <summary>
    /// Gets or sets the logger instance used for logging in the module.
    /// </summary>
    public ILogger Logger { get; set; } = null!;

    /// <summary>
    /// Gets the user who initiated the interaction.
    /// </summary>
    public SocketUser Caller => Context.Interaction.User;

    /// <summary>
    /// Gets the bot's user information.
    /// </summary>
    public SocketSelfUser BotUser => Context.Client.CurrentUser;

    /// <summary>
    /// Gets or sets the rate limiter for controlling command execution frequency.
    /// </summary>
    public RateLimiter RateLimiter { get; set; } = null!;

    /// <summary>
    /// Gets or sets the service for retrieving assembly information.
    /// </summary>
    public IAssemblyService AssemblyService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the service for accessing environment-related information.
    /// </summary>
    public IEnvironmentService EnvironmentService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the translation service for localizing bot responses.
    /// </summary>
    public ITranslationService TranslationService { get; set; } = null!;

    #endregion

    #region Response Methods

    /// <summary>
    /// Sends a response or follow-up message to the user in direct messages (DM).
    /// If the interaction was initiated in a DM, it responds there; otherwise, it sends a DM to the user.
    /// </summary>
    /// <param name="text">The text content of the message.</param>
    /// <param name="isTTS">Indicates whether the message should be read aloud using text-to-speech.</param>
    /// <param name="embed">The embed content to include in the message.</param>
    /// <param name="options">Additional options for the message.</param>
    /// <param name="allowedMentions">Specifies who can be mentioned in the message.</param>
    /// <param name="messageReference">References a specific message in the conversation.</param>
    /// <param name="components">Interactive components (e.g., buttons) to include with the message.</param>
    /// <param name="stickers">Stickers to include in the message.</param>
    /// <param name="embeds">An array of embeds to include in the message.</param>
    /// <param name="flags">Message flags for the interaction.</param>
    /// <param name="ephemeral">Indicates whether the response should only be visible to the user.</param>
    protected async Task RespondOrFollowupInDMAsync(
        string? text = null,
        bool isTTS = false,
        Embed? embed = null,
        RequestOptions? options = null,
        AllowedMentions? allowedMentions = null,
        MessageReference? messageReference = null,
        MessageComponent? components = null,
        ISticker[]? stickers = null,
        Embed[]? embeds = null,
        MessageFlags flags = MessageFlags.None,
        bool ephemeral = false)
    {
        try
        {
            if (Context.Channel is IDMChannel)
            {
                await RespondOrFollowupAsync(
                    text: text,
                    isTTS: isTTS,
                    embed: embed,
                    options: options,
                    allowedMentions: allowedMentions,
                    components: components,
                    embeds: embeds,
                    ephemeral: ephemeral);
            }
            else
            {
                await RespondOrFollowupAsync(TranslationService.GetString("follow-up-in-DM"), ephemeral: true);
                IDMChannel dm = await Caller.CreateDMChannelAsync();
                await dm.SendMessageAsync(
                    text: text,
                    isTTS: isTTS,
                    embed: embed,
                    options: options,
                    allowedMentions: allowedMentions,
                    messageReference: messageReference,
                    components: components,
                    stickers: stickers,
                    embeds: embeds,
                    flags: flags);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to send DM response.");
            await RespondOrFollowupAsync(TranslationService.GetString("error-sending-DM"));
        }
    }

    /// <summary>
    /// Sends a response or follow-up message based on whether the interaction has already been responded to.
    /// </summary>
    /// <param name="text">The text content of the message.</param>
    /// <param name="isTTS">Indicates whether the message should be read aloud using text-to-speech.</param>
    /// <param name="embed">The embed content to include in the message.</param>
    /// <param name="options">Additional options for the message.</param>
    /// <param name="allowedMentions">Specifies who can be mentioned in the message.</param>
    /// <param name="components">Interactive components (e.g., buttons) to include with the message.</param>
    /// <param name="embeds">An array of embeds to include in the message.</param>
    /// <param name="ephemeral">Indicates whether the response should only be visible to the user.</param>
    protected async Task RespondOrFollowupAsync(
        string? text = null,
        bool isTTS = false,
        Embed? embed = null,
        RequestOptions? options = null,
        AllowedMentions? allowedMentions = null,
        MessageComponent? components = null,
        Embed[]? embeds = null,
        bool ephemeral = false)
    {
        try
        {
            if (Context.Interaction.HasResponded)
            {
                await FollowupAsync(
                    text: text,
                    embeds: embeds,
                    isTTS: isTTS,
                    allowedMentions: allowedMentions,
                    options: options,
                    components: components,
                    embed: embed);
            }
            else
            {
                await RespondAsync(
                    text: text,
                    embeds: embeds,
                    isTTS: isTTS,
                    ephemeral: ephemeral,
                    allowedMentions: allowedMentions,
                    options: options,
                    components: components,
                    embed: embed);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to respond or follow up.");
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a new embed builder with preset properties such as the author, timestamp, and footer.
    /// </summary>
    /// <returns>A configured <see cref="EmbedBuilder"/> instance.</returns>
    protected EmbedBuilder GetEmbedBuilder() => new()
    {
        Author = new EmbedAuthorBuilder
        {
            Name = Caller.Username,
            IconUrl = Caller.GetAvatarUrl()
        },
        Color = Color.Gold,
        Timestamp = DateTimeOffset.UtcNow,
        Footer = new EmbedFooterBuilder
        {
            IconUrl = BotUser.GetAvatarUrl(),
            Text = TranslationService.GetAttrString("bot", "name", TranslationHelper.Arguments("version", AssemblyService.Version, "environment", EnvironmentService.EnvironmentName))
        }
    };

    /// <summary>
    /// Checks if the current user is allowed to execute the specified command based on the rate limit settings.
    /// </summary>
    /// <param name="commandName">The name of the command method. Automatically derived from the calling method's name when not specified explicitly.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing true if the user is allowed to execute the command, false otherwise.</returns>
    protected async Task<bool> CheckRateLimitAsync([CallerMemberName] string commandName = "")
    {
        MethodInfo? method = GetType().GetMethod(commandName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        RateLimitAttribute? rateLimitAttribute = method?.GetCustomAttribute<RateLimitAttribute>();

        if (rateLimitAttribute != null)
        {
            ulong userId = Context.User.Id;
            Logger.Debug("Checking rate limit [{MaxAttempts}/{TimeWindow}] for user {UserId} on command {CommandName}", rateLimitAttribute.MaxAttempts, rateLimitAttribute.TimeWindow, userId, commandName);

            if (!await RateLimiter.IsAllowed(userId, commandName, rateLimitAttribute.MaxAttempts, rateLimitAttribute.TimeWindow))
            {
                await ReplyAsync(TranslationService.GetString("error-rate-limit"));
                return false;
            }
        }

        return true;
    }

    #endregion
}

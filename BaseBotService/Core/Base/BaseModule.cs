using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Attributes;
using Discord.WebSocket;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BaseBotService.Core.Base;
/// <summary>
/// Base class for all modules that provide interaction-based commands.
/// </summary>
public abstract class BaseModule : InteractionModuleBase<SocketInteractionContext>
{
    /// <summary>
    /// The logger for the current module.
    /// </summary>
    public ILogger Logger { get; set; } = null!;
    /// <summary>
    /// The user that invoked the command.
    /// </summary>
    public SocketUser Caller => Context.Interaction.User;
    /// <summary>
    /// The guild in which the command was invoked.
    /// </summary>
    public ulong? GuildId => Context.Interaction.GuildId;
    /// <summary>
    /// The bot user instance.
    /// </summary>
    public SocketSelfUser BotUser => Context.Client.CurrentUser;
    /// <summary>
    /// The rate limiter instance to be used.
    /// </summary>
    public RateLimiter RateLimiter { get; set; } = null!;
    /// <summary>
    /// Provides information about available assemblies.
    /// </summary>
    public IAssemblyService AssemblyService { get; set; } = null!;
    /// <summary>
    /// The environment information instance.
    /// </summary>
    public IEnvironmentService EnvironmentService { get; set; } = null!;
    /// <summary>
    /// Provides translation services.
    /// </summary>
    public ITranslationService TranslationService { get; set; } = null!;

    /// <summary>
    /// Sends a reply or a follow-up in a private message.
    /// </summary>
    /// <param name="text">The text to send.</param>
    /// <param name="isTTS">Whether or not the message should be text-to-speech enabled.</param>
    /// <param name="embed">The embed object to send.</param>
    /// <param name="options">The request options to use.</param>
    /// <param name="allowedMentions">The allowed mentions properties for the message.</param>
    /// <param name="messageReference">The message to reference.</param>
    /// <param name="components">The message components to include.</param>
    /// <param name="stickers">Message stickers to include.</param>
    /// <param name="embeds">Embed objects to include.</param>
    /// <param name="flags">Flags to include.</param>
    /// <param name="ephemeral">Whether or not the message should be ephemeral.</param>
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

    /// <summary>
    /// Sends a reply or a follow-up in the current context.
    /// </summary>
    /// <param name="text">The text to send.</param>
    /// <param name="isTTS">Whether or not the message should be text-to-speech enabled.</param>
    /// <param name="embed">The embed object to send.</param>
    /// <param name="options">The request options to use.</param>
    /// <param name="allowedMentions">The allowed mentions properties for the message.</param>
    /// <param name="components">The message components to include.</param>
    /// <param name="embeds">Embed objects to include.</param>
    /// <param name="ephemeral">Whether or not the message should be ephemeral.</param>
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

    /// <summary>
    /// Gets an <see cref="EmbedBuilder"/> instance for use.
    /// </summary>
    /// <returns>An <see cref="EmbedBuilder"/> instance.</returns>
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
    /// <example>
    /// <code>
    /// public class MySlashCommandModule : BaseBotService.Modules.BaseModule
    /// {
    ///     [SlashCommand("example", "An example slash command.")]
    ///     [RateLimit(5, 60)] // Limit to 5 calls per 60 seconds
    ///     public async Task ExampleCommandAsync()
    ///     {
    ///         if (!await CheckRateLimitAsync())
    ///         {
    ///             return;
    ///         }
    ///
    ///         // Command implementation
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <param name="commandName"></param>
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
}

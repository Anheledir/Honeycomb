using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Attributes;
using Discord.WebSocket;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BaseBotService.Core.Base;
public abstract class BaseModule : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection,
    // public properties with public setters will be set by the service provider
    public ILogger Logger { get; set; } = null!;
    public SocketUser Caller => Context.Interaction.User;
    public SocketSelfUser BotUser => Context.Client.CurrentUser;
    public RateLimiter RateLimiter { get; set; } = null!;
    public IAssemblyService AssemblyService { get; set; } = null!;
    public IEnvironmentService EnvironmentService { get; set; } = null!;

    protected async Task FollowupInDMAsync(
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
        if (Context.Channel is not IDMChannel)
        {
            await FollowupAsync("You've got a DM!", ephemeral: true);
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
        else
        {
            await FollowupAsync(
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
            Text = $"Honeycomb v{AssemblyService.Version} ({EnvironmentService.EnvironmentName})"
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
                await ReplyAsync("You have reached the rate limit for this command. Please wait before trying again.");
                return false;
            }
        }

        return true;
    }
}

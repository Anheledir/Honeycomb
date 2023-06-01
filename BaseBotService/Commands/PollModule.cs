using BaseBotService.Commands.Modals;
using BaseBotService.Core.Base;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using Discord.Rest;

namespace BaseBotService.Commands;

// TODO: Add role check for guild moderators

[Group("polls", "Create and manage polls for your server.")]
[EnabledInDm(false)]
[RequireContext(ContextType.Guild)]
public class PollModule : BaseModule
{
    private readonly IPollRepository _pollRepository;
    private readonly IGuildRepository _guildRepository;

    public PollModule(ILogger logger, IGuildRepository guildRepository, IPollRepository pollRepository)
    {
        Logger = logger.ForContext<PollModule>();
        _pollRepository = pollRepository;
        _guildRepository = guildRepository;
    }

    [SlashCommand("create", "Create a new poll in this server.")]
    public async Task CreatePoll()
    {
        // The default channel is the current one
        IChannel channel = Context.Channel;

        // Check if the channel is a text channel
        if (channel is not ITextChannel)
        {
            Logger.Information($"User {Caller.Id} tried to create a new poll on {GuildId} in {channel.Id}, which is not a text channel.");
            await RespondOrFollowupAsync(TranslationService.GetString("error-poll-create-invalid-channel"), ephemeral: true);
            return;
        }

        // Check if the bot has permissions to send messages in the channel
        ChannelPermissions usrPermissions = (Caller as IGuildUser).GetPermissions(channel as IGuildChannel);
        if (!usrPermissions.SendMessages)
        {
            Logger.Information($"Cannot create a new poll, as the calling user does not have permissions to create messages in {channel.Id} on {GuildId}.");
            await RespondOrFollowupAsync(TranslationService.GetString("error-poll-create-no-permissions"), ephemeral: true);
        }

        var modal = new ModalBuilder()
            .WithCustomId("polls.create")
            .WithTitle(TranslationService.GetAttrString("polls", "create"))
            .AddTextInput(TranslationService.GetAttrString("polls", "create-poll-title"), "title", TextInputStyle.Short, minLength: 3, required: true)
            .AddTextInput(TranslationService.GetAttrString("polls", "create-poll-description"), "description", TextInputStyle.Paragraph, required: false);
        await RespondWithModalAsync(modal.Build());
    }

    [ModalInteraction("polls.create", ignoreGroupNames: true)]
    public async Task CreateNewPollWizard(PollsCreateNewModal data)
    {
        RestUserMessage pollMessage = await Context.Channel.SendMessageAsync(TranslationService.GetAttrString("polls", "create-msg-processing"));

        _pollRepository.AddPoll(new PollHC
        {
            PollId = pollMessage.Id,
            Title = data.Title,
            Description = data.Description,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
        });

        var buttons = new ComponentBuilder()
            .WithButton("Add Option", "poll-add-option", ButtonStyle.Primary)
            .WithButton("Public Results", "poll-results-toggle", ButtonStyle.Secondary)
            .WithButton("Show Voters", "poll-voters-toggle", ButtonStyle.Secondary)
            .WithButton("Finish", "poll-complete", ButtonStyle.Success)
            .WithButton("Cancel", "poll-cancel", ButtonStyle.Danger);
        // Respond with interaction buttons to add options, change the duration, toggle visibility of results and voters, finish the poll, and cancel

        await RespondOrFollowupAsync("Please finish your poll...", components: buttons.Build(), ephemeral: true);
    }

}

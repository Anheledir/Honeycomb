using BaseBotService.Commands.Modals;
using BaseBotService.Core.Base;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Utilities.Extensions;
using Discord.Rest;
using Discord.WebSocket;

namespace BaseBotService.Commands;

// TODO: Add role check for guild moderators

[Group("polls", "Create and manage polls for your server.")]
[EnabledInDm(false)]
[RequireContext(ContextType.Guild)]
public class PollModule : BaseModule
{
    private readonly IPollRepository _pollRepository;

    public PollModule(ILogger logger, IPollRepository pollRepository)
    {
        Logger = logger.ForContext<PollModule>();
        _pollRepository = pollRepository;
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

        var pollData = new PollHC
        {
            PollId = pollMessage.Id,
            ChannelId = Context.Channel.Id,
            GuildId = Context.Guild.Id,
            CreatorId = Caller.Id,
            Title = data.Title,
            Description = data.Description,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            IsClosed = false
        };
        _pollRepository.AddPoll(pollData);

        await RespondOrFollowupAsync("Please finish setting up your poll...", embed: GetCreatePollEmbed(pollData).Build(), components: GetCreatePollButtons(pollMessage.Id).Build(), ephemeral: true);
    }

    [ComponentInteraction("polls.create.toggle.results:*", ignoreGroupNames: true)]
    public async Task CreateNewPollToggleResults(string pollId)
    {
        PollHC? newPoll = await GetPollData(pollId);
        if (newPoll == null) return;

        newPoll.AreResultsPublic = !newPoll.AreResultsPublic;
        _pollRepository.UpdatePoll(newPoll);

        _ = await ModifyOriginalResponseAsync(msg => msg.Embed = GetCreatePollEmbed(newPoll).Build());
    }

    [ComponentInteraction("polls.create.toggle.voters:*", ignoreGroupNames: true)]
    public async Task CreateNewPollToggleVoters(string pollId)
    {
        PollHC? newPoll = await GetPollData(pollId);
        if (newPoll == null) return;

        newPoll.AreVotersHidden = !newPoll.AreVotersHidden;
        _pollRepository.UpdatePoll(newPoll);

        _ = await ModifyOriginalResponseAsync(msg => msg.Embed = GetCreatePollEmbed(newPoll).Build());
    }

    [ComponentInteraction("polls.create.finish:*")]
    public async Task CreateNewPollFinish(string pollId)
    {
        PollHC? newPoll = await GetPollData(pollId);
        if (newPoll == null) return;

        var message = await GetPollMessage(newPoll);
        var pollEmbed = GetCreatePollEmbed(newPoll);
        pollEmbed.Url = message.GetJumpUrl();

        await message.ModifyAsync(msg => msg.Embed = pollEmbed.Build());
        await message.PinAsync();
        await DeleteOriginalResponseAsync();
    }

    [ComponentInteraction("polls.create.cancel:*")]
    public async Task CreateNewPollCancel(string pollId)
    {
        PollHC? newPoll = await GetPollData(pollId);
        if (newPoll == null) return;

        _pollRepository.DeletePoll(newPoll.PollId);

        var message = await GetPollMessage(newPoll);
        await message.DeleteAsync();

        await ModifyOriginalResponseAsync(msg => msg.Content = "Poll creation was aborted.");
    }

    private async Task<PollHC?> GetPollData(string pollId)
    {
        PollHC? newPoll = _pollRepository.GetPoll(ulong.Parse(pollId), false);
        if (newPoll == null)
        {
            _ = await ModifyOriginalResponseAsync(msg => msg.Content = TranslationService.GetString("error-poll-create-invalid-poll"));
            return null;
        }
        return newPoll;
    }

    private ComponentBuilder GetCreatePollButtons(ulong pollId) => new ComponentBuilder()
        .WithButton("Add Option", $"polls.create.option.add:{pollId}", ButtonStyle.Primary)
        .WithButton("Delete Option", $"polls.create.option.delete:{pollId}", ButtonStyle.Primary)
        .WithButton("Toggle Public Results", $"polls.create.toggle.results:{pollId}", ButtonStyle.Secondary)
        .WithButton("Toggle Show Voters", $"polls.create.toggle.voters:{pollId}", ButtonStyle.Secondary)
        .WithButton("Change Duration", $"polls.create.duration:{pollId}", ButtonStyle.Secondary)
        .WithButton("Finish", $"polls.create.finish:{pollId}", ButtonStyle.Success)
        .WithButton("Cancel", $"polls.create.cancel:{pollId}", ButtonStyle.Danger);

    private EmbedBuilder GetCreatePollEmbed(PollHC poll) =>
        new EmbedBuilder()
            .WithTitle(poll.Title)
            .WithDescription(poll.Description)
            // TODO: Add options with current votes
            .WithColor(Color.Blue)
            .WithFooter($"Poll created by {Context.Guild.GetUser(poll.CreatorId).Username}, open until {poll.EndDate.ToDiscordTimestamp(TranslationService)}")
            .WithTimestamp(poll.StartDate);

    private async Task<IUserMessage?> GetPollMessage(PollHC? newPoll) =>
        await (Context.Guild.GetChannel(newPoll.ChannelId) as SocketTextChannel).GetMessageAsync(newPoll.PollId) as IUserMessage;
}

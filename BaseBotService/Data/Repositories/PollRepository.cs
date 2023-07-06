using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data.Repositories;

public class PollRepository(ILiteCollection<PollHC> polls, ILiteCollection<PollOptionsHC> options, ILiteCollection<PollVotesHC> votes) : IPollRepository
{
    public PollHC? GetPoll(ulong pollId, bool create = false)
    {
        PollHC? result = polls
      .Include(o => o.Options)
      .Include(v => v.Votes)
      .FindOne(p => p.PollId == pollId);

        if (create && result == null)
        {
            result = new PollHC { PollId = pollId };
            polls.Insert(result);
        }

        return result;
    }

    public void AddPoll(PollHC poll) => polls.Insert(poll);

    public bool UpdatePoll(PollHC poll) => polls.Update(poll);

    public bool DeletePoll(ulong pollId)
    {
        var user = GetPoll(pollId);
        if (user != null)
        {
            return polls.Delete(user.Id);
        }
        return false;
    }

    public ObjectId AddPollOption(PollHC poll, string emoji, string name, int order = 0)
    {
        var newOption = new PollOptionsHC { PollId = poll.PollId, Emoji = emoji, Text = name, Order = order };
        options.Insert(newOption);
        poll.Options.Add(newOption);
        UpdatePoll(poll);
        return newOption.Id;
    }

    public ObjectId AddPollVote(PollHC poll, string optionId, ulong voterId)
    {
        var optionBsonId = new ObjectId(optionId);

        // Delete the existing vote of the user
        poll.Votes.RemoveAll(v => v.PollId == poll.PollId && v.VoterId == voterId);
        UpdatePoll(poll);
        _ = votes.DeleteMany(v => v.PollId == poll.PollId && v.VoterId == voterId);

        // Check if option exists
        _ =
            options.FindOne(o => o.PollId == poll.PollId && o.Id == optionBsonId)
            ?? throw new ArgumentException("Option does not exist");

        // Check if poll is still open
        if (poll.IsClosed || poll.EndDate < DateTime.UtcNow || poll.StartDate > DateTime.UtcNow)
        {
            throw new ArgumentException("Poll is closed");
        }

        var newVote = new PollVotesHC { PollId = poll.PollId, OptionId = optionBsonId, VotedAt = DateTime.UtcNow, VoterId = voterId };
        votes.Insert(newVote);
        poll.Votes.Add(newVote);
        UpdatePoll(poll);
        return newVote.Id;
    }
}
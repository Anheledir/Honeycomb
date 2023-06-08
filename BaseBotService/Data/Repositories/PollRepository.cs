using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data.Repositories;

public class PollRepository : IPollRepository
{
    private readonly ILiteCollection<PollHC> _polls;

    public PollRepository(ILiteCollection<PollHC> polls) => _polls = polls;

    public PollHC? GetPoll(ulong pollId, bool create = false)
    {
        PollHC? result = _polls
            .Include(o => o.Options)
            .Include(v => v.Votes)
            .FindOne(p => p.PollId == pollId);
        if (create && result == null)
        {
            _polls.Insert(new PollHC { PollId = pollId });
            result = _polls.FindOne(p => p.PollId == pollId);
        }
        return result;
    }

    public void AddPoll(PollHC poll) => _polls.Insert(poll);

    public bool UpdatePoll(PollHC poll) => _polls.Update(poll);

    public bool DeletePoll(ulong pollId)
    {
        var user = GetPoll(pollId);
        if (user != null)
        {
            return _polls.Delete(user.Id);
        }
        return false;
    }
}
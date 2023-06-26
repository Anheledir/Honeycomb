using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data.Repositories;

public class MemberRepository(ILiteCollection<MemberHC> members) : IMemberRepository
{
    public MemberHC? GetUser(ulong userId, bool create = false)
    {
        MemberHC? result = members
            .Include(a => a.Achievements)
            .FindOne(a => a.MemberId == userId);
        if (create && result == null)
        {
            members.Insert(new MemberHC { MemberId = userId });
            result = members.FindOne(a => a.MemberId == userId);
        }
        return result;
    }

    public void AddUser(MemberHC user) => members.Insert(user);

    public bool UpdateUser(MemberHC user) => members.Update(user);

    public bool DeleteUser(ulong userId)
    {
        var user = GetUser(userId);
        if (user != null)
        {
            return members.Delete(user.Id);
        }
        return false;
    }
}
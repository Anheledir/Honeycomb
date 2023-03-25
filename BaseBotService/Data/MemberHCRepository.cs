using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data;

public class MemberHCRepository : IMemberHCRepository
{
    private readonly ILiteCollection<MemberHC> _members;

    public MemberHCRepository(ILiteCollection<MemberHC> members)
    {
        _members = members;
    }

    public MemberHC GetUser(ulong userId)
    {
        return _members.FindOne(a => a.MemberId == userId);
    }

    public void AddUser(MemberHC user)
    {
        _members.Insert(user);
    }

    public bool UpdateUser(MemberHC user)
    {
        return _members.Update(user);
    }

    public bool DeleteUser(ulong userId)
    {
        var user = GetUser(userId);
        if (user != null)
        {
            return _members.Delete(user.Id);
        }
        return false;
    }
}
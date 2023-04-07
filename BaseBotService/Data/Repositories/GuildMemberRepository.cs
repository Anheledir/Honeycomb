using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data.Repositories;

public class GuildMemberRepository : IGuildMemberRepository
{
    private readonly ILiteCollection<GuildMemberHC> _guildMembers;

    public GuildMemberRepository(ILiteCollection<GuildMemberHC> guildMembers) => _guildMembers = guildMembers;

    public GuildMemberHC GetUser(ulong guildId, ulong userId)
    {
        return _guildMembers.FindOne(a => a.GuildId == guildId && a.MemberId == userId);
    }

    public void AddUser(GuildMemberHC user) => _guildMembers.Insert(user);

    public bool UpdateUser(GuildMemberHC user)
    => _guildMembers.Update(user);

    public bool DeleteUser(ulong guildId, ulong userId)
    {
        var user = GetUser(guildId, userId);
        if (user != null)
        {
            return _guildMembers.Delete(user.Id);
        }
        return false;
    }
}
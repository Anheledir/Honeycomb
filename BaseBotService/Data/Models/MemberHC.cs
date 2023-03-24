using BaseBotService.Core.Base;
using LiteDB;

namespace BaseBotService.Data.Models;
public class MemberHC : HCModelBase
{
    public ulong MemberId { get; set; }
    public DateTime? Birthday { get; set; }

    public static ILiteCollection<MemberHC> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<MemberHC> collection = GetServiceRegistration<MemberHC>(services);
        _ = collection.EnsureIndex(x => x.MemberId);
        return collection;
    }
}

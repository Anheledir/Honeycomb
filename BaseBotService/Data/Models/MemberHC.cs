using BaseBotService.Commands.Enums;
using BaseBotService.Core.Base;
using LiteDB;

namespace BaseBotService.Data.Models;
public class MemberHC : HCModelBase
{
    public ulong MemberId { get; set; }
    public DateTime? Birthday { get; set; }
    public Countries Country { get; set; } = Countries.Unknown;
    public Languages Languages { get; set; } = Languages.Other;
    public Timezone Timezone { get; set; } = Timezone.GMT;
    public List<HCAchievementBase> Achievements { get; set; } = new();
    public GenderIdentity GenderIdentity { get; set; } = GenderIdentity.Unknown;

    public static ILiteCollection<MemberHC> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<MemberHC> collection = GetServiceRegistration<MemberHC>(services);
        _ = collection.EnsureIndex(x => x.MemberId);
        return collection;
    }
}

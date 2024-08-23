using BaseBotService.Commands.Enums;
using BaseBotService.Core.Base;
using BaseBotService.Data.Interfaces;
using LiteDB;

namespace BaseBotService.Data.Models;

public class MemberHC : ModelBase
{
    public ulong MemberId { get; set; }
    public DateTime? Birthday { get; set; }
    public Countries Country { get; set; } = Countries.Unknown;
    public Languages Languages { get; set; } = Languages.Other;
    public Timezone Timezone { get; set; } = Timezone.GMT;
    public List<AchievementBase> Achievements { get; set; } = new();
    public GenderIdentity GenderIdentity { get; set; } = GenderIdentity.Unknown;

    // Additional methods for specia modules
    public List<T> GetAchievements<T>(IAchievementRepository<T> repository) where T : AchievementBase
       => repository.GetByUserId(MemberId).FindAll(a => a.SourceIdentifier == repository.Identifier).ToList();


    // Ensure that the MemberId field is indexed
    protected override void EnsureIndexes<T>(ILiteCollection<T> collection)
    {
        var memberCollection = collection as ILiteCollection<MemberHC>;
        memberCollection?.EnsureIndex(x => x.MemberId, unique: true);
    }
}

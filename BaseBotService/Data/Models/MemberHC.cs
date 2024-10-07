using BaseBotService.Commands.Enums;
using BaseBotService.Core.Base;
using BaseBotService.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BaseBotService.Data.Models;

public class MemberHC
{
    [Key]
    public ulong MemberId { get; set; }

    public DateTime? Birthday { get; set; }
    public Countries Country { get; set; } = Countries.Unknown;
    public Languages Languages { get; set; } = Languages.Other;
    public Timezone Timezone { get; set; } = Timezone.GMT;
    public List<AchievementBase> Achievements { get; set; } = new();
    public GenderIdentity GenderIdentity { get; set; } = GenderIdentity.Unknown;

    public async Task<List<T>> GetAchievementsAsync<T>(IAchievementRepository<T> repository) where T : AchievementBase
    {
        var achievements = await repository.GetByUserIdAsync(MemberId);
        return achievements.Where(a => a.SourceIdentifier == repository.Identifier).ToList();
    }
}

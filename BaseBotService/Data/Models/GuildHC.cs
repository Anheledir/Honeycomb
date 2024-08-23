using BaseBotService.Core.Base;
using BaseBotService.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace BaseBotService.Data.Models;

public class GuildHC
{
    [Key]
    public ulong GuildId { get; set; }

    public GuildSettings Settings { get; set; }
    public string? ActivityPointsName { get; set; }
    public string? ActivityPointsSymbol { get; set; }
    public double ActivityPointsAverageActiveHours { get; set; }
    public List<ulong> ModeratorRoles { get; set; }
    public List<ulong> ArtistRoles { get; set; }
    public List<AchievementBase> Achievements { get; set; } = new();
    public List<GuildMemberHC> Members { get; set; } = new();
    public ulong InternalNotificationChannel { get; set; } = 0;

    public GuildHC()
    {
        Settings = GuildSettings.EnableActivityPoints
            | GuildSettings.EnableUserProfilesGenderIdentity
            | GuildSettings.EnableUserProfilesBirthday
            | GuildSettings.EnablePublicUserProfiles
            | GuildSettings.EnableGlobalEvents;
        ModeratorRoles = new List<ulong>();
        ArtistRoles = new List<ulong>();
        ActivityPointsAverageActiveHours = 4;
    }
}

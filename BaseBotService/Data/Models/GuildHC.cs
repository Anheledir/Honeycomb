using BaseBotService.Core.Base;
using BaseBotService.Data.Enums;
using LiteDB;

namespace BaseBotService.Data.Models;

public class GuildHC : ModelBase
{
    public ulong GuildId { get; set; }
    public GuildSettings Settings { get; set; }
    public string? ActivityPointsName { get; set; }
    public string? ActivityPointsSymbol { get; set; }
    public double ActivityPointsAverageActiveHours { get; set; }
    public List<ulong> ModeratorRoles { get; set; }
    public List<ulong> ArtistRoles { get; set; }
    public List<GuildMemberHC> Members { get; set; } = new();
    public ulong InternalNotificationChannel { get; set; } = 0;

    // Constructor to initialize default values
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

    // Override the EnsureIndexes method to set up the GuildId index
    protected override void EnsureIndexes<T>(ILiteCollection<T> collection)
    {
        var guildCollection = collection as ILiteCollection<GuildHC>;
        guildCollection?.EnsureIndex(x => x.GuildId, unique: true);
    }
}

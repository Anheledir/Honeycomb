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

    public static ILiteCollection<GuildHC> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<GuildHC> collection = GetServiceRegistration<GuildHC>(services);
        _ = collection.EnsureIndex(x => x.GuildId, unique: true);
        return collection;
    }
}

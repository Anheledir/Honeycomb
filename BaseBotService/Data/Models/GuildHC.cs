using BaseBotService.Core.Base;
using BaseBotService.Data.Enums;
using LiteDB;

namespace BaseBotService.Data.Models;
public class GuildHC : HCModelBase
{
    public ulong GuildId { get; set; }
    public GuildSettings Settings { get; set; }
    public string? ActivityPointsName { get; set; }
    public string? ActivityPointsSymbol { get; set; }
    public double ActivityPointsAverageActiveHours { get; set; }
    public List<ulong> ModeratorRoles { get; set; }
    public List<ulong> ArtistRoles { get; set; }

    public GuildHC()
    {
        Settings = GuildSettings.EnableActivityPoints | GuildSettings.EnableUserProfilesGenderIdentity | GuildSettings.EnableUserProfilesBirthday | GuildSettings.EnablePublicUserProfiles;
        ModeratorRoles = new List<ulong>();
        ArtistRoles = new List<ulong>();
    }

    public static ILiteCollection<GuildHC> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<GuildHC> collection = GetServiceRegistration<GuildHC>(services);
        _ = collection.EnsureIndex(x => x.GuildId);
        return collection;
    }
}

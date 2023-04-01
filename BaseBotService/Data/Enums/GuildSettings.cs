namespace BaseBotService.Data.Enums;

[Flags]
public enum GuildSettings
{
    None = 0,
    EnableActivityPoints = 1 << 0,
    EnableUserProfilesGenderIdentity = 1 << 1,
    EnableUserProfilesBirthday = 1 << 2,
    EnablePublicUserProfiles = 1 << 3,
}
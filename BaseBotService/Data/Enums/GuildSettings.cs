namespace BaseBotService.Data.Enums;

/// <summary>
/// Represents the guild settings as a bitwise flag enumeration.
/// </summary>
[Flags]
public enum GuildSettings
{
    /// <summary>
    /// No settings enabled.
    /// </summary>
    None = 0,

    /// <summary>
    /// Enables activity points tracking.
    /// </summary>
    EnableActivityPoints = 1 << 0,

    /// <summary>
    /// Enables gender identity in user profiles.
    /// </summary>
    EnableUserProfilesGenderIdentity = 1 << 1,

    /// <summary>
    /// Enables birthday information in user profiles.
    /// </summary>
    EnableUserProfilesBirthday = 1 << 2,

    /// <summary>
    /// Enables public visibility for user profiles.
    /// </summary>
    EnablePublicUserProfiles = 1 << 3,
}

namespace BaseBotService.Commands.Enums;
/// <summary>
/// Represents the gender identity options for a Discord user.
/// </summary>
public enum GenderIdentity
{
    /// <summary>
    /// Not specified.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Refers to individuals who identify as male.
    /// </summary>
    Male = 1,

    /// <summary>
    /// Refers to individuals who identify as female.
    /// </summary>
    Female = 2,

    /// <summary>
    /// Refers to individuals who identify as neither entirely male nor entirely female, or as a combination of both male and female.
    /// </summary>
    NonBinary = 3,

    /// <summary>
    /// Refers to individuals who were assigned a female sex at birth but identify as male.
    /// </summary>
    TransgenderMale = 4,

    /// <summary>
    /// Refers to individuals who were assigned a male sex at birth but identify as female.
    /// </summary>
    TransgenderFemale = 5,

    /// <summary>
    /// Refers to individuals who do not conform to traditional gender norms and may identify as a combination of genders or as a third gender.
    /// </summary>
    Genderqueer = 6,

    /// <summary>
    /// Refers to individuals who do not identify with any of the above options or prefer not to disclose their gender identity.
    /// </summary>
    Other = 7
}

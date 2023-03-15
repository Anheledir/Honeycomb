namespace BaseBotService.Enumeration;
/// <summary>
/// Represents the gender identity options for a Discord user.
/// </summary>
public enum GenderIdentity
{
    /// <summary>
    /// Refers to individuals who identify as male.
    /// </summary>
    Male,

    /// <summary>
    /// Refers to individuals who identify as female.
    /// </summary>
    Female,

    /// <summary>
    /// Refers to individuals who identify as neither entirely male nor entirely female, or as a combination of both male and female.
    /// </summary>
    NonBinary,

    /// <summary>
    /// Refers to individuals who were assigned a female sex at birth but identify as male.
    /// </summary>
    TransgenderMale,

    /// <summary>
    /// Refers to individuals who were assigned a male sex at birth but identify as female.
    /// </summary>
    TransgenderFemale,

    /// <summary>
    /// Refers to individuals who do not conform to traditional gender norms and may identify as a combination of genders or as a third gender.
    /// </summary>
    Genderqueer,

    /// <summary>
    /// Refers to individuals who do not identify with any of the above options or prefer not to disclose their gender identity.
    /// </summary>
    Other
}

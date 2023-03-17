using BaseBotService.Enumeration;
using BaseBotService.Extensions;

namespace BaseBotService.Tests.Extensions;
public class GenderIdentitiesTests
{
    [TestCase(GenderIdentity.Male, ":male_sign: Male")]
    [TestCase(GenderIdentity.Female, ":female_sign: Female")]
    [TestCase(GenderIdentity.NonBinary, ":transgender_symbol: NonBinary")]
    [TestCase(GenderIdentity.TransgenderMale, ":transgender_flag: TransgenderMale")]
    [TestCase(GenderIdentity.TransgenderFemale, ":transgender_flag: TransgenderFemale")]
    [TestCase(GenderIdentity.Genderqueer, ":transgender_symbol: Genderqueer")]
    [TestCase(GenderIdentity.Other, ":grey_question: Other")]
    public void GetGenderNameWithFlag_ShouldReturnCorrectNameWithFlag(GenderIdentity gender, string expected)
    {
        string actual = gender.GetFlaggedGenderName();
        Assert.That(actual, Is.EqualTo(expected));
    }
}

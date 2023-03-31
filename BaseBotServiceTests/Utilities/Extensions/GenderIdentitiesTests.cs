using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities.Extensions;

namespace BaseBotService.Tests.Utilities.Extensions;
public class GenderIdentitiesTests
{
    private ITranslationService _translationService;

    [SetUp]
    public void SetUp()
    {
        _translationService = Substitute.For<ITranslationService>();
    }

    [TestCase(GenderIdentity.Male, ":male_sign: Male")]
    [TestCase(GenderIdentity.Female, ":female_sign: Female")]
    [TestCase(GenderIdentity.NonBinary, ":transgender_symbol: Non Binary")]
    [TestCase(GenderIdentity.TransgenderMale, ":transgender_flag: Transgender Male")]
    [TestCase(GenderIdentity.TransgenderFemale, ":transgender_flag: Transgender Female")]
    [TestCase(GenderIdentity.Genderqueer, ":transgender_symbol: Genderqueer")]
    [TestCase(GenderIdentity.Other, ":grey_question: Other")]
    public void GetGenderNameWithFlag_ShouldReturnCorrectNameWithFlag(GenderIdentity gender, string expected)
    {
        // Arrange
        string id = $"gender-{gender.ToString().ToLowerKebabCase()}";
        _translationService.GetString(id).Returns(gender.ToString().FromCamelCase());

        // Act
        string actual = gender.GetFlaggedGenderName(_translationService);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}

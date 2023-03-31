using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities.Extensions;

namespace BaseBotService.Tests.Utilities.Extensions;

[TestFixture]
public class CountryLanguageExtensionsTests
{
    private ITranslationService _translationService;

    [SetUp]
    public void SetUp()
    {
        _translationService = Substitute.For<ITranslationService>();
    }

    [TestCase(Languages.English, ":flag_gb: English")]
    [TestCase(Languages.German, ":flag_de: German")]
    [TestCase(Languages.French, ":flag_fr: French")]
    public void GetFlaggedLanguageName_ShouldReturnCorrectNameWithFlag(Languages language, string expected)
    {
        string id = $"language-{language.ToString().ToLowerKebabCase()}";
        _translationService.GetString(id).Returns(language.ToString().FromCamelCase());
        string actual = language.GetFlaggedLanguageNames(_translationService);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetFlaggedLanguageNames_ShouldReturnCorrectNamesWithFlags_WhenLanguagesAreCombinationOfMultipleLanguages()
    {
        // Arrange
        const Languages languages = Languages.English | Languages.German | Languages.French;
        _translationService.GetString("language-english").Returns("English");
        _translationService.GetString("language-german").Returns("German");
        _translationService.GetString("language-french").Returns("French");

        // Act
        string actual = languages.GetFlaggedLanguageNames(_translationService);

        // Assert
        const string expected = ":flag_gb: English\n:flag_de: German\n:flag_fr: French";
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetFlaggedLanguageNames_ShouldReturnOther_WhenLanguagesAreUnknown()
    {
        // Arrange
        const Languages languages = Languages.Other;
        _translationService.GetString("language-other").Returns("Other");

        // Act
        string actual = languages.GetFlaggedLanguageNames(_translationService);

        // Assert
        const string expected = "Other";
        Assert.That(actual, Is.EqualTo(expected));
    }
}
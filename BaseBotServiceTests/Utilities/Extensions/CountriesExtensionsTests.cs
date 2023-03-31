using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities.Extensions;

namespace BaseBotService.Tests.Utilities.Extensions;

[TestFixture]
public class CountriesExtensionsTests
{
    [Test]
    public void GetCountryNameWithFlag_ShouldReturnCountryNameWithFlag_WhenCountryHasFlag()
    {
        // Arrange
        const Countries country = Countries.UnitedStates;
        var translationService = Substitute.For<ITranslationService>();
        translationService.GetString("country-united-states").Returns("United States");

        // Act
        string result = country.GetCountryNameWithFlag(translationService);

        // Assert
        result.ShouldBe(":flag_us: United States");
    }

    [Test]
    public void GetCountryNameWithFlag_ShouldReturnCountryNameWithoutFlag_WhenCountryDoesNotHaveFlag()
    {
        // Arrange
        const Countries country = Countries.Unknown;
        var translationService = Substitute.For<ITranslationService>();

        // Act
        string result = country.GetCountryNameWithFlag(translationService);

        // Assert
        result.ShouldBe("Unknown");
    }

    [Test]
    public void GetCountryNameWithFlag_ShouldReturnFormattedCountryName_WhenCountryNameIsCamelCase()
    {
        // Arrange
        const Countries country = Countries.SouthKorea;
        var translationService = Substitute.For<ITranslationService>();
        translationService.GetString("country-south-korea").Returns("South Korea");

        // Act
        string result = country.GetCountryNameWithFlag(translationService);

        // Assert
        result.ShouldBe(":flag_kr: South Korea");
    }

    [Test]
    public void GetCountryNameWithFlag_ShouldReturnFormattedCountryName_WhenCountryNameHasAbbreviation()
    {
        // Arrange
        const Countries country = Countries.Swiss;
        var translationService = Substitute.For<ITranslationService>();
        translationService.GetString("country-swiss").Returns("Swiss");

        // Act
        string result = country.GetCountryNameWithFlag(translationService);

        // Assert
        result.ShouldBe(":flag_ch: Swiss");
    }
}
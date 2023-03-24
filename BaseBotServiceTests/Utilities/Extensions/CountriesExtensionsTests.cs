using BaseBotService.Interactions.Enums;
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

        // Act
        string result = country.GetCountryNameWithFlag();

        // Assert
        result.ShouldBe(":flag_us: United States");
    }

    [Test]
    public void GetCountryNameWithFlag_ShouldReturnCountryNameWithoutFlag_WhenCountryDoesNotHaveFlag()
    {
        // Arrange
        const Countries country = Countries.Unknown;

        // Act
        string result = country.GetCountryNameWithFlag();

        // Assert
        result.ShouldBe("Unknown");
    }

    [Test]
    public void GetCountryNameWithFlag_ShouldReturnFormattedCountryName_WhenCountryNameIsCamelCase()
    {
        // Arrange
        const Countries country = Countries.SouthKorea;

        // Act
        string result = country.GetCountryNameWithFlag();

        // Assert
        result.ShouldBe(":flag_kr: South Korea");
    }

    [Test]
    public void GetCountryNameWithFlag_ShouldReturnFormattedCountryName_WhenCountryNameHasAbbreviation()
    {
        // Arrange
        const Countries country = Countries.Swiss;

        // Act
        string result = country.GetCountryNameWithFlag();

        // Assert
        result.ShouldBe(":flag_ch: Swiss");
    }
}
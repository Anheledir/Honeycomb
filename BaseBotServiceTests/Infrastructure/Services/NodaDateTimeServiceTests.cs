using BaseBotService.Infrastructure.Services;

namespace BaseBotService.Tests.Infrastructure.Services;

[TestFixture]
public class NodaDateTimeServiceTests
{
    private NodaDateTimeService _dateTimeService;

    [SetUp]
    public void SetUp()
    {
        _dateTimeService = new NodaDateTimeService();
    }

    [TestCase(2022, 4, 17)]
    [TestCase(2023, 4, 9)]
    [TestCase(2024, 3, 31)]
    public void IsEasterSunday_ShouldReturnTrueForEasterSunday(int year, int expectedMonth, int expectedDay)
    {
        // Arrange
        LocalDate expectedEasterSundayDate = new(year, expectedMonth, expectedDay);

        // Act
        bool isEasterSunday = _dateTimeService.IsEasterSunday(expectedEasterSundayDate);

        // Assert
        isEasterSunday.ShouldBeTrue($"The date {expectedEasterSundayDate} should be Easter Sunday for the year {year}");
    }

    [TestCase(2022, 4, 18)]
    [TestCase(2023, 4, 10)]
    [TestCase(2024, 4, 1)]
    public void IsEasterMonday_ShouldReturnTrueForEasterMonday(int year, int expectedMonth, int expectedDay)
    {
        // Arrange
        LocalDate expectedEasterMondayDate = new(year, expectedMonth, expectedDay);

        // Act
        bool isEasterMonday = _dateTimeService.IsEasterMonday(expectedEasterMondayDate);

        // Assert
        isEasterMonday.ShouldBeTrue($"The date {expectedEasterMondayDate} should be Easter Monday for the year {year}");
    }

    [TestCase(2020, 4, 12)]
    [TestCase(2021, 4, 4)]
    [TestCase(2022, 4, 17)]
    [TestCase(2023, 4, 9)]
    [TestCase(2024, 3, 31)]
    public void GetEasterDate_ReturnsCorrectEasterDate(int year, int expectedMonth, int expectedDay)
    {
        // Arrange
        LocalDate expectedDate = new(year, expectedMonth, expectedDay);

        // Act
        LocalDate easterDate = NodaDateTimeService.GetEasterDate(year);

        // Assert
        easterDate.ShouldBe(expectedDate);
    }
}

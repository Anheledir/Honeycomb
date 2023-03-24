using BaseBotService.Utilities.Extensions;
using BaseBotService.Interactions.Enums;

namespace BaseBotService.Tests.Utilities.Extensions;

[TestFixture]
public class TimezoneExtensionsTests
{
    [TestCase(Timezone.GMT, "GMT (+00:00)")]
    [TestCase(Timezone.PST, "PST (-08:00)")]
    [TestCase(Timezone.EST, "EST (-05:00)")]
    [TestCase(Timezone.CET, "CET (+01:00)")]
    public void ToString_ShouldReturnCorrectTimezoneString(Timezone timezone, string expectedTimezoneString)
    {
        // Act
        string actualTimezoneString = timezone.GetNameWithOffset();

        // Assert
        actualTimezoneString.ShouldBe(expectedTimezoneString);
    }
}
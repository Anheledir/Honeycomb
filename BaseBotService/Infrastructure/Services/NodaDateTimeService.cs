using BaseBotService.Core.Interfaces;
using System.Collections.Concurrent;

namespace BaseBotService.Infrastructure.Services;

public class NodaDateTimeService : IDateTimeProvider
{
    // Optional: Cache to store calculated Easter dates for performance
    private static readonly ConcurrentDictionary<int, LocalDate> EasterDateCache = new();

    public Instant GetCurrentInstant() => SystemClock.Instance.GetCurrentInstant();

    public ZonedDateTime ConvertToZonedDateTime(Instant instant, DateTimeZone timeZone)
    {
        if (timeZone == null) throw new ArgumentNullException(nameof(timeZone));
        return instant.InZone(timeZone);
    }

    public Instant ConvertToInstant(ZonedDateTime zonedDateTime) => zonedDateTime.ToInstant();

    public Instant ConvertToInstant(LocalDateTime localDateTime, DateTimeZone timeZone)
    {
        if (timeZone == null) throw new ArgumentNullException(nameof(timeZone));
        return localDateTime.InZoneLeniently(timeZone).ToInstant();
    }

    public Instant ConvertToInstant(DateTimeOffset dateTimeOffset) => Instant.FromDateTimeOffset(dateTimeOffset);

    public LocalDateTime ConvertToLocalDateTime(Instant instant, DateTimeZone timeZone)
    {
        if (timeZone == null) throw new ArgumentNullException(nameof(timeZone));
        return instant.InZone(timeZone).LocalDateTime;
    }

    public DateTimeOffset ConvertToDateTimeOffset(Instant instant) => instant.ToDateTimeOffset();

    public bool IsEasterSunday(LocalDate date) => date == GetEasterDate(date.Year);

    public bool IsEasterMonday(LocalDate date) => date == GetEasterDate(date.Year).PlusDays(1);

    internal static LocalDate GetEasterDate(int year)
    {
        // Optional: Use cached value if available
        return EasterDateCache.GetOrAdd(year, CalculateEasterDate);
    }

    /// <summary>
    /// Calculates the date of Easter for a given year using the Anonymous Gregorian Algorithm.
    /// </summary>
    /// <param name="year">The year for which to calculate the Easter date.</param>
    /// <returns>The date of Easter for the specified year.</returns>
    /// <remarks>
    /// Easter is always the first Sunday after the first full moon on or after the vernal equinox.
    /// </remarks>
    private static LocalDate CalculateEasterDate(int year)
    {
        int yearFactor = year % 19;
        int century = year / 100;
        int yearInCentury = year % 100;
        int centuryLeapYears = century / 4;
        int centuryNonLeapYears = century % 4;
        int centuryOffset = (century + 8) / 25;
        int magicNumber = (century - centuryOffset + 1) / 3;
        int moonOffset = ((19 * yearFactor) + century - centuryLeapYears - magicNumber + 15) % 30;
        int centuryLeapOffset = yearInCentury / 4;
        int centuryNonLeapOffset = yearInCentury % 4;
        int weekDayOffset = (32 + (2 * centuryNonLeapYears) + (2 * centuryLeapOffset) - moonOffset - centuryNonLeapOffset) % 7;
        int leapYearOffset = (yearFactor + (11 * moonOffset) + (22 * weekDayOffset)) / 451;

        int month = (moonOffset + weekDayOffset - (7 * leapYearOffset) + 114) / 31;
        int day = ((moonOffset + weekDayOffset - (7 * leapYearOffset) + 114) % 31) + 1;

        return new LocalDate(year, month, day);
    }

    public LocalDate GetCurrentUtcDate() => ConvertToLocalDateTime(GetCurrentInstant(), DateTimeZone.Utc).Date;
}

namespace BaseBotService.Core.Interfaces;

/// <summary>
/// Provides an interface for working with date and time, allowing interaction with users
/// from different time zones and organization of events spanning multiple time zones or days.
/// This interface can also be used for mocking date and time in unit tests.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current instant in time.
    /// </summary>
    /// <returns>An Instant representing the current point in time.</returns>
    Instant GetCurrentInstant();

    /// <summary>
    /// Gets the current UTC date as a LocalDate.
    /// </summary>
    /// <returns>
    /// The current UTC date as a LocalDate, representing only the date part (year, month, and day) without the time component.
    /// </returns>
    LocalDate GetCurrentUtcDate();

    /// <summary>
    /// Converts an instant to a ZonedDateTime in the specified time zone.
    /// </summary>
    /// <param name="instant">The Instant to convert.</param>
    /// <param name="timeZone">The target time zone for the conversion.</param>
    /// <returns>A ZonedDateTime in the specified time zone.</returns>
    ZonedDateTime ConvertToZonedDateTime(Instant instant, DateTimeZone timeZone);

    /// <summary>
    /// Converts a ZonedDateTime to an Instant.
    /// </summary>
    /// <param name="zonedDateTime">The ZonedDateTime to convert.</param>
    /// <returns>An Instant representing the same point in time as the ZonedDateTime.</returns>
    Instant ConvertToInstant(ZonedDateTime zonedDateTime);

    /// <summary>
    /// Converts a LocalDateTime to an Instant in the specified time zone.
    /// </summary>
    /// <param name="localDateTime">The LocalDateTime to convert.</param>
    /// <param name="timeZone">The time zone of the LocalDateTime.</param>
    /// <returns>An Instant representing the same point in time as the LocalDateTime in the specified time zone.</returns>
    Instant ConvertToInstant(LocalDateTime localDateTime, DateTimeZone timeZone);

    /// <summary>
    /// Converts a DateTimeOffset to an Instant.
    /// </summary>
    /// <param name="dateTimeOffset">The DateTimeOffset to convert.</param>
    /// <returns>An Instant representing the same point in time as the DateTimeOffset.</returns>
    Instant ConvertToInstant(DateTimeOffset dateTimeOffset);

    /// <summary>
    /// Converts an instant to a LocalDateTime in the specified time zone.
    /// </summary>
    /// <param name="instant">The Instant to convert.</param>
    /// <param name="timeZone">The target time zone for the conversion.</param>
    /// <returns>A LocalDateTime in the specified time zone.</returns>
    LocalDateTime ConvertToLocalDateTime(Instant instant, DateTimeZone timeZone);

    /// <summary>
    /// Converts an Instant to a DateTimeOffset.
    /// </summary>
    /// <param name="instant">The Instant to convert.</param>
    /// <returns>A DateTimeOffset representing the same point in time as the Instant.</returns>
    DateTimeOffset ConvertToDateTimeOffset(Instant instant);

    /// <summary>
    /// Determines if the given LocalDate is Easter Sunday for the specified year.
    /// </summary>
    /// <param name="date">The LocalDate to check.</param>
    /// <returns>True if the date is Easter Sunday, false otherwise.</returns>
    bool IsEasterSunday(LocalDate date);

    /// <summary>
    /// Determines if the given LocalDate is Easter Monday for the specified year.
    /// </summary>
    /// <param name="date">The LocalDate to check.</param>
    /// <returns>True if the date is Easter Monday, false otherwise.</returns>
    bool IsEasterMonday(LocalDate date);
}
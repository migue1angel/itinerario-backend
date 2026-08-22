namespace Api.Services;

public sealed class PortTimeConverter
{
    public DateTimeOffset ToPortLocalTime(DateTimeOffset instantUtc, string TimeZone)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
        return TimeZoneInfo.ConvertTime(instantUtc, timeZone);
    }

    public bool HasMatchingOffset(
    DateTimeOffset dateTime,
    string timeZoneId)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        var localDateTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);

        return localDateTime.Offset == dateTime.Offset;
    }

    public bool IsWithinOperatingWindow(DateTimeOffset departureAt, string timeZoneId)
    {
        var localTime = ToPortLocalTime(departureAt, timeZoneId);
        return localTime.TimeOfDay >= TimeSpan.FromHours(6)
            && localTime.TimeOfDay <= TimeSpan.FromHours(18);
    }
}

namespace InventorySystem.Application.Common.Time;

public static class BaghdadTime
{
    private static readonly TimeZoneInfo TimeZone =
        ResolveTimeZone();

    public static DateTime Now =>
        TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZone);

    public static DateTime FromUtc(
        DateTime utcDateTime)
    {
        var normalizedUtc =
            utcDateTime.Kind == DateTimeKind.Utc
                ? utcDateTime
                : DateTime.SpecifyKind(
                    utcDateTime,
                    DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(
            normalizedUtc,
            TimeZone);
    }

    public static DateTime? FromUtc(
        DateTime? utcDateTime)
    {
        if (!utcDateTime.HasValue)
            return null;

        return FromUtc(
            utcDateTime.Value);
    }

    private static TimeZoneInfo ResolveTimeZone()
    {
        var ids =
            new[]
            {
                "Asia/Baghdad",
                "Arabic Standard Time"
            };

        foreach (var id in ids)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(
                    id);
            }
            catch (
                TimeZoneNotFoundException)
            {
            }
            catch (
                InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.CreateCustomTimeZone(
            "Baghdad",
            TimeSpan.FromHours(3),
            "Baghdad",
            "Baghdad");
    }
}

using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Domain.Utils;

public static class Constants
{
    public static HourFrequency GetWeekType(DateTime date)
    {
        int weeks = (DateOnly.FromDateTime(date).DayNumber - HardcodedData.CalendarStartDate.DayNumber) / 7;

        return weeks % 2 == 0 ? HourFrequency.FirstWeek : HourFrequency.SecondWeek;
    }

    public const int DefaultStringMaxLenght = 100;
    public const int ExtendedStringMaxLenght = 255;
    public const string CalendarTzId = "Europe/Bucharest";

    public static HourFrequency CurrentWeekType => GetWeekType(DateTime.Now);
}

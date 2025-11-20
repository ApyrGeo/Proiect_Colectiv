namespace TrackForUBB.Domain.Enums;

public enum HourDay
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

public class HourDayConverter
{
    public static DayOfWeek ConvertToDayOfWeek(HourDay hourDay)
    {
        return hourDay switch
        {
            HourDay.Monday => DayOfWeek.Monday,
            HourDay.Tuesday => DayOfWeek.Tuesday,
            HourDay.Wednesday => DayOfWeek.Wednesday,
            HourDay.Thursday => DayOfWeek.Thursday,
            HourDay.Friday => DayOfWeek.Friday,
            HourDay.Saturday => DayOfWeek.Saturday,
            HourDay.Sunday => DayOfWeek.Sunday,
            _ => throw new ArgumentOutOfRangeException(nameof(hourDay), hourDay, null)
        };
    }
}
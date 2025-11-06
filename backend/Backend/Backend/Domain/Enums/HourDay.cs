namespace Backend.Domain.Enums;

public enum HourDay
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5
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
            _ => throw new ArgumentOutOfRangeException(nameof(hourDay), hourDay, null)
        };
    }
}
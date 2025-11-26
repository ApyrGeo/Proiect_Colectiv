using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Domain.Utils;
public class HelperFunctions
{
    public static HourFrequency GetWeekType(DateTime date)
    {
        int weeks = (DateOnly.FromDateTime(date).DayNumber - HardcodedData.CalendarStartDate.DayNumber) / 7;

        return weeks % 2 == 0 ? HourFrequency.FirstWeek : HourFrequency.SecondWeek;
    }

    public static HourFrequency CurrentWeekType => GetWeekType(DateTime.Now);

    public static int GetCurrentStudentYear(int promotionStartYear)
    {
        int currentYear = DateTime.Now.Year;
        int yearDifference = currentYear - promotionStartYear;
        if (DateTime.Now.Month >= 7)
            yearDifference += 1;

        return yearDifference;
    }
}
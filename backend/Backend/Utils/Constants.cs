using Domain.Enums;

namespace Utils;

public static class Constants
{
    public const int DefaultStringMaxLenght = 100;
    public const int ExtendedStringMaxLenght = 255;
    public const string CalendarTzId = "Europe/Bucharest";

    public static HourFrequency CurrentWeekType => HourHelper.GetWeekType(DateTime.Now);
}

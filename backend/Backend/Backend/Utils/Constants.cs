using Backend.Domain.Enums;

namespace Backend.Utils;

public static class Constants
{
    public const int DefaultStringMaxLenght = 100;
    public const int ExtendedStringMaxLenght = 255;

    public static HourFrequency CurrentWeekType => HourHelper.GetWeekType(DateTime.Now);
}

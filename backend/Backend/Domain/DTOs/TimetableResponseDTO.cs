using System.Globalization;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Domain.DTOs;

public class TimetableResponseDTO
{
    public List<HourResponseDTO> Hours { get; set; } = [];

    public string CalendarStartISODate { get; set; } = HardcodedData.CalendarStartDate.ToString("o", CultureInfo.InvariantCulture);
}

using System.Globalization;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Domain.DTOs;

public class TimetableResponseDTO
{
    public List<HourResponseDTO> Hours { get; set; } = [];

    public required string CalendarStartISODate { get; set; }
}

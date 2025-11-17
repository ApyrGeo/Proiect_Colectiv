using Backend.Utils;
using System.Globalization;

namespace Backend.Domain.DTOs;

public class TimetableResponseDTO
{
    public List<HourResponseDTO> Hours { get; set; } = [];

    public string CalendarStartISODate => HardcodedData.CalendarStartDate.ToString("o", CultureInfo.InvariantCulture);
}

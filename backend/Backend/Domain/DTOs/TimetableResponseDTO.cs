namespace Domain.DTOs;

public class TimetableResponseDTO
{
    public List<HourResponseDTO> Hours { get; set; } = [];

    public string CalendarStartISODate { get; set; } = string.Empty;
}

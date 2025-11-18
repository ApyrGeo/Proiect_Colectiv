namespace Domain.DTOs;

public class HourPostDTO
{
    public string? Day { get; set; }
    public string? HourInterval { get; set; }
    public string? Frequency { get; set; }
    public string? Category { get; set; }
    public required int ClassroomId { get; set; }
    public required int SubjectId { get; set; }
    public required int TeacherId { get; set; }

    public int? GroupYearId { get; set; }
    public int? StudentGroupId { get; set; }
    public int? StudentSubGroupId { get; set; }
}

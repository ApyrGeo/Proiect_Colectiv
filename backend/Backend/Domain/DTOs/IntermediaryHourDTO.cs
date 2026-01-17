using TrackForUBB.Domain.Enums;

namespace TrackForUBB.Domain.DTOs;

public class IntermediaryHourDTO
{
    public int Id { get; set; }
    public HourDay Day { get; set; }
    public string? HourInterval { get; set; }
    public HourFrequency Frequency { get; set; }
    public HourCategory Category { get; set; }
    public int? ClassroomId { get; set; }
    public required int SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public int? GroupYearId { get; set; }
    public int? StudentGroupId { get; set; }
    public int? StudentSubGroupId { get; set; }
}

namespace TrackForUBB.Domain.Utils;

public class HourFilter
{
    public int? UserId { get; set; } = null;
    public int? ClassroomId { get; set; } = null;
    public int? SubjectId { get; set; } = null;
    public int? TeacherId { get; set; } = null;
    public int? FacultyId { get; set; } = null;
    public int? SpecialisationId { get; set; } = null;
    public int? GroupYearId { get; set; } = null;
    public bool? CurrentWeekTimetable { get; set; } = null;
}

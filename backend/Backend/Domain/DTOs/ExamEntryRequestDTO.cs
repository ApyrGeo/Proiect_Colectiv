namespace TrackForUBB.Domain.DTOs;

public class ExamEntryRequestDTO
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int StudentGroupId { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? Duration { get; set; }
    public int? ClassroomId { get; set; }
}

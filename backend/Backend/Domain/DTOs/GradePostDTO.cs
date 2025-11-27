namespace TrackForUBB.Domain.DTOs;

public class GradePostDTO
{
    public int Value { get; set; }
    public required int SubjectId { get; set; }
    public int SemesterId { get; set; }
    public int EnrollmentId { get; set; }
}
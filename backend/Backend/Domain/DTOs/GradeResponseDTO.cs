namespace TrackForUBB.Domain.DTOs;

public class GradeResponseDTO
{
    public required int Id { get; set; }
    public required int Value { get; set; }
    public required SubjectResponseDTO Subject { get; set; }
    public required PromotionSemesterResponseDTO Semester { get; set; }
    public required EnrollmentResponseDTO Enrollment { get; set; }
}
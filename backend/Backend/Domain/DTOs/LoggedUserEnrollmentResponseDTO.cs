namespace TrackForUBB.Domain.DTOs;

public class LoggedUserEnrollmentResponseDTO
{
    public required int EnrollmentId { get; set; }
    public required int SubGroupId { get; set; }
    public required int GroupId { get; set; }
    public required int PromotionId { get; set; }
    public required int SpecializationId { get; set; }
    public required int FacultyId { get; set; }
}

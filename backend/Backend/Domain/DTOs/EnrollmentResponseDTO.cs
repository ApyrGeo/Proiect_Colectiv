namespace Domain.DTOs;

public class EnrollmentResponseDTO
{
    public required int Id { get; set; }
    public required int UserId { get; set; }
    public required UserResponseDTO User { get; set; }
    public required int SubGroupId { get; set; }
    public required StudentSubGroupResponseDTO SubGroup { get; set; }
}

namespace Backend.Domain.DTOs;

public class EnrollmentResponseDTO
{
    public required int Id { get; set; }
    public required int UserId { get; set; }
    public UserResponseDTO User { get; set; } = null!; 
    public required int SubGroupId { get; set; }
    public StudentSubGroupResponseDTO SubGroup { get; set; } = null!; 
}

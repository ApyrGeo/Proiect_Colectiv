namespace Backend.Domain.DTOs;

public class TeacherResponseDTO
{
    public required int Id { get; set; }
    public required int UserId { get; set; }
    public required UserResponseDTO User { get; set; }
    public required int FacultyId { get; set; }
}

namespace TrackForUBB.Domain.DTOs;

public class LoggedUserResponseDTO
{
    public required UserResponseDTO User { get; set; }
    public List<LoggedUserEnrollmentResponseDTO> Enrollments { get; set; } = [];
}

namespace TrackForUBB.Domain.DTOs;

public class LoggedUserResponseDTO
{
    public required UserResponseDTO User { get; set; }
    public List<LoggedUserEnrollmentResponseDTO> Enrollemnts { get; set; } = [];
}

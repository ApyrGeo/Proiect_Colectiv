namespace TrackForUBB.Domain.DTOs;

public class UserGradeDTO
{
    public required SimplifiedUserResponseDTO User { get; set; }
    public required GradeResponseDTO Grade { get; set; }
}
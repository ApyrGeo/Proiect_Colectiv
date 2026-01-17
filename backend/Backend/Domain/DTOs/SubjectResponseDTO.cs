namespace TrackForUBB.Domain.DTOs;

public class SubjectResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int NumberOfCredits { get; set; }
    public required string Code { get; set; }
    public required string Type { get; set; }
}

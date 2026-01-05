namespace TrackForUBB.Domain.DTOs;

public class SubjectPostDTO
{
    public string? Name { get; set; }
    public required int NumberOfCredits { get; set; }
    public int HolderTeacherId { get; set; }
    public required string Code { get; set; }
    public required int SemesterId { get; set; }
    public required string Type { get; set; }
}

namespace TrackForUBB.Domain.DTOs;

public class OptionalPackageResponseDTO
{
    public int PackageId { get; set; }
    public int SemesterNumber { get; set; }
    public required int Semester1or2 { get; set; }
    public required int YearNumber { get; set; }
    public List<SubjectResponseDTO> Subjects { get; set; } = [];
}

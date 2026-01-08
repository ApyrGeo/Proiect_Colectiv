namespace TrackForUBB.Domain.DTOs;

public class OptionalPackageResponseDTO
{
    public int PackageId { get; set; }
    public int SemesterNumber { get; set; }
    public List<SubjectResponseDTO> Subjects { get; set; } = [];
}

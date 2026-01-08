namespace TrackForUBB.Domain.DTOs;

public class OptionalPackageResponseDTO
{
    public int PackageId { get; set; }
    public List<SubjectResponseDTO> Subjects { get; set; } = [];
}
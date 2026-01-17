namespace TrackForUBB.Domain.DTOs;

public class FacultyResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public List<SpecialisationResponseDTO> Specialisations { get; set; } = [];
}

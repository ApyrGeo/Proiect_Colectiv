using Newtonsoft.Json;

namespace Backend.Domain.DTOs;

[JsonObject(IsReference = true)]
public class FacultyResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public List<SpecialisationResponseDTO> Specialisations { get; set; } = [];
}

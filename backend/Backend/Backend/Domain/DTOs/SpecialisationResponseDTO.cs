using Newtonsoft.Json;

namespace Backend.Domain.DTOs;

[JsonObject(IsReference = true)]
public class SpecialisationResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public List<GroupYearResponseDTO> GroupYears { get; set; } = [];
    public FacultyResponseDTO? Faculty { get; set; }
}
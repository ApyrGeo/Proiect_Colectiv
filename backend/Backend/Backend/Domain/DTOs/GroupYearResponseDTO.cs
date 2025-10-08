using Newtonsoft.Json;

namespace Backend.Domain.DTOs;

[JsonObject(IsReference = true)]
public class GroupYearResponseDTO
{
    public required int Id { get; set; }
    public required string Year { get; set; }

    public SpecialisationResponseDTO? Specialisation { get; set; }
    public List<StudentGroupResponseDTO>? StudentGroups { get; set; }
}
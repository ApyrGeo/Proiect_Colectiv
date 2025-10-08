using Newtonsoft.Json;

namespace Backend.Domain.DTOs;

[JsonObject(IsReference = true)]
public class StudentGroupResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public GroupYearResponseDTO? GroupYear { get; set; }
    public List<StudentSubGroupResponseDTO> StudentSubGroups { get; set; } = [];
}
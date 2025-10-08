using Newtonsoft.Json;

namespace Backend.Domain.DTOs;

[JsonObject(IsReference = true)]
public class StudentSubGroupResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public StudentGroupResponseDTO? StudentGroup { get; set; }
}
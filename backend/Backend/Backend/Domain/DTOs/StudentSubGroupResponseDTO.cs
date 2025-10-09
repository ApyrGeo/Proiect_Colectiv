using Newtonsoft.Json;

namespace Backend.Domain.DTOs;

public class StudentSubGroupResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}
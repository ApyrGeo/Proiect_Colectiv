namespace Backend.Domain.DTOs;

public class StudentGroupResponseDTO
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public List<StudentSubGroupResponseDTO> StudentSubGroups { get; set; } = [];
}
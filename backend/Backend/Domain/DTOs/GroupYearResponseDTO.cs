namespace Domain.DTOs;

public class GroupYearResponseDTO
{
    public required int Id { get; set; }
    public required string Year { get; set; }

    public List<StudentGroupResponseDTO> StudentGroups { get; set; } = [];
}
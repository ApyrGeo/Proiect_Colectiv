namespace Domain.DTOs;

public class StudentGroupPostDTO
{
    public string? Name { get; set; }
    public required int GroupYearId { get; set; }
}
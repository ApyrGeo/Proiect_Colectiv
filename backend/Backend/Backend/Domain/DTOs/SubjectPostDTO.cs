namespace Backend.Domain.DTOs;

public class SubjectPostDTO
{
    public required string Name { get; set; }
    public required int NumberOfCredits { get; set; }
    public required int GroupYearId { get; set; }
}
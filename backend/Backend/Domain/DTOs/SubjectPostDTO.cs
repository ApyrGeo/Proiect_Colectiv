namespace Domain.DTOs;

public class SubjectPostDTO
{
    public string? Name { get; set; }
    public required int NumberOfCredits { get; set; }
    public required int GroupYearId { get; set; }
}
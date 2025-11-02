namespace Backend.Domain.DTOs;

public class SubjectPostDTO
{
    public required string Name { get; set; }
    public required int NrCredits { get; set; }
    public  bool ForScholarship { get; set; }
}
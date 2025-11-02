namespace Backend.Domain.Enums;

public class Subject
{       
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int NrCredits { get; set; }
    public required bool ForScholarship { get; set; }
}
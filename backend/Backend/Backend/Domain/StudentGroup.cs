namespace Backend.Domain;

public class StudentGroup
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required List<StudentSubGroup> StudentSubGroups { get; set; }
    public required int GroupYearId { get; set; }
    public required GroupYear GroupYear { get; set; }
}
namespace Backend.Domain;

public class StudentGroup
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<StudentSubGroup> StudentSubGroups { get; set; }
    public int GroupYearId { get; set; }
    public required GroupYear GroupYear { get; set; }
}
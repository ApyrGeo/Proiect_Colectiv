namespace Backend.Domain;

public class GroupYear
{
    public required int Id { get; set; }
    public required string Year { get; set; }
    public required List<StudentGroup> StudentGroups { get; set; }
    public required int SpecialisationId { get; set; }
    public required Specialisation Specialisation { get; set; }
}
namespace TrackForUBB.Repository.EFEntities;

public class GroupYear
{
    public int Id { get; set; }
    public required string Year { get; set; }
    public List<StudentGroup> StudentGroups { get; set; } = [];
    public int SpecialisationId { get; set; }
    public required Specialisation Specialisation { get; set; }
    public List<Subject> Subjects { get; set; } = [];
}
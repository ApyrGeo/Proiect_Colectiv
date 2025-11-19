namespace TrackForUBB.Repository.EFEntities;

public class StudentSubGroup
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Enrollment> Enrollments { get; set; } = [];
    public int StudentGroupId { get; set; }
    public required StudentGroup StudentGroup { get; set; }
}
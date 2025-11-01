namespace Backend.Domain;

public class StudentSubGroup
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required List<Enrollment> Enrollments { get; set; } = [];
    public required int StudentGroupId { get; set; }
    public required StudentGroup StudentGroup { get; set; }
}
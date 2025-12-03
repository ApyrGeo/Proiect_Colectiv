namespace TrackForUBB.Repository.EFEntities;

public class Teacher
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int FacultyId { get; set; }
    public required Faculty Faculty { get; set; }
    public List<Hour> Hours { get; set; } = [];

    // Here we will pretend that a teacher can hold more subjects
    public int? HeldSubjectId { get; set; }
    public Subject? HeldSubject { get; set; } = null;
}

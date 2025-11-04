namespace Backend.Domain;

public class Teacher
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int FacultyId { get; set; }
    public required Faculty Faculty { get; set; }
    public List<Hour> Hours { get; set; } = [];
}

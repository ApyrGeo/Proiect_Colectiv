namespace Backend.Domain;

public class Enrollment
{
    public required int Id { get; set; }
    public required int UserId { get; set; }
    public required User User { get; set; }
    public required int SubGroupId { get; set; }
    public required StudentSubGroup SubGroup { get; set; }
}

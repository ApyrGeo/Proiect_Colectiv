namespace TrackForUBB.Domain;

public class Enrollment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int SubGroupId { get; set; }
    public required StudentSubGroup SubGroup { get; set; }
}

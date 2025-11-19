namespace TrackForUBB.Domain;

public class Specialisation
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<GroupYear> GroupYears { get; set; } = [];
    public int FacultyId { get; set; }
    public required Faculty Faculty { get; set; }
}
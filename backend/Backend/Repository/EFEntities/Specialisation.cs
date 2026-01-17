namespace TrackForUBB.Repository.EFEntities;

public class Specialisation
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Promotion> Promotions { get; set; } = [];
    public int FacultyId { get; set; }
    public required Faculty Faculty { get; set; }
}
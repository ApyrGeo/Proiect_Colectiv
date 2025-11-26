namespace TrackForUBB.Repository.EFEntities;

public class Promotion
{
    public int Id { get; set; }
    public required int StartYear { get; set; }
    public required int EndYear { get; set; }
    public List<StudentGroup> StudentGroups { get; set; } = [];
    public int SpecialisationId { get; set; }
    public required Specialisation Specialisation { get; set; }
    public List<PromotionYear> Years { get; set; } = [];
}
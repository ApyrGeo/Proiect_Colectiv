namespace TrackForUBB.Repository.EFEntities;

public class Subject
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int NumberOfCredits { get; set; }
    public required int GroupYearId { get; set; }
    public required GroupYear GroupYear { get; set; }
    public List<Hour> Hours { get; set; } = [];
}
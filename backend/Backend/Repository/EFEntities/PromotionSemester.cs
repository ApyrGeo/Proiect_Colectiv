namespace TrackForUBB.Repository.EFEntities;

public class PromotionSemester
{
	public int Id { get; set; }
	public required int SemesterNumber { get; set; }
	public int PromotionId { get; set; }
	public required Promotion Promotion { get; set; }
	public List<Contract> Contracts { get; set; } = [];
    public List<Subject> Subjects { get; set; } = [];
}

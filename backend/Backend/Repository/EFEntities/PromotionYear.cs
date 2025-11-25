namespace TrackForUBB.Repository.EFEntities;

public class PromotionYear
{
	public int Id { get; set; }
	public int YearNumber { get; set; }
	public int PromotionId { get; set; }
	public required Promotion Promotion { get; set; }
	public List<PromotionSemester> PromotionSemesters { get; set; } = [];
}
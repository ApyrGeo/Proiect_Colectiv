namespace TrackForUBB.Repository.EFEntities;

public class PromotionSemester
{
	public int Id { get; set; }
	public required int SemesterNumber { get; set; }
	public int PromotionYearId { get; set; }
	public required PromotionYear PromotionYear { get; set; }
	public List<Grade> Grades { get; set; } = [];
	public List<Hour> Hours { get; set; } = [];
	public List<Contract> Contracts { get; set; } = [];
}